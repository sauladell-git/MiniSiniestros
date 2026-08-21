using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Services.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace MiniSiniestros.Services.Implementations
{
    public class SrtNotificationClient : ISrtNotificationClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SrtNotificationClient> _logger;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

        public SrtNotificationClient(HttpClient httpClient, ILogger<SrtNotificationClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configuración explícita de la política resiliente de Polly v8 (Retry + Circuit Breaker + Timeout)
            _resiliencePipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                // 1. Retry Strategy: 3 reintentos exponenciales con Jitter
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(r => !r.IsSuccessStatusCode)
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>(),
                    OnRetry = args =>
                    {
                        _logger.LogWarning("[POLLY-RETRY] Reintento {AttemptNumber} de 3 tras fallo ({Outcome}). Esperando {Delay}...",
                            args.AttemptNumber + 1, args.Outcome.Exception?.Message ?? $"HTTP {args.Outcome.Result?.StatusCode}", args.RetryDelay);
                        return ValueTask.CompletedTask;
                    }
                })
                // 2. Circuit Breaker Strategy: Abre tras 2 fallos consecutivos por 10s
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(10),
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(r => !r.IsSuccessStatusCode)
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>(),
                    OnOpened = args =>
                    {
                        _logger.LogError(" [POLLY-CIRCUIT-BREAKER] ¡Circuito ABIERTO durante {Duration}s debido a fallos continuos!", args.BreakDuration.TotalSeconds);
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        _logger.LogInformation(" [POLLY-CIRCUIT-BREAKER] Circuito CERRADO. Servicio SRT restablecido.");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        _logger.LogInformation(" [POLLY-CIRCUIT-BREAKER] Circuito MEDIO-ABIERTO. Probando respuesta del servicio SRT...");
                        return ValueTask.CompletedTask;
                    }
                })
                // 3. Timeout Strategy: 3 segundos máximo por intento
                .AddTimeout(TimeSpan.FromSeconds(3))
                .Build();
        }

        public async Task<SrtNotificationOutcomeDto> NotificarAprobacionAsync(SrtPayloadDto payload, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(" [POLLY-CLIENT] Ejecutando política de resiliencia Polly para Siniestro ID: {SiniestroId}...", payload.SiniestroId);

            int intentosTotales = 0;

            try
            {
                // Ejecución protegida de la petición HTTP con el pipeline de Polly
                var response = await _resiliencePipeline.ExecuteAsync(async (ct) =>
                {
                    intentosTotales++;
                    return await _httpClient.PostAsJsonAsync("srt-mock/notificar", payload, ct);
                }, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    string? codigoTx = null;

                    try
                    {
                        using var doc = JsonDocument.Parse(jsonContent);
                        if (doc.RootElement.TryGetProperty("codigoTransaccion", out var txProp))
                        {
                            codigoTx = txProp.GetString();
                        }
                    }
                    catch { }

                    _logger.LogInformation("✅ [POLLY-CLIENT] Notificación entregada exitosamente a la SRT (SiniestroId: {SiniestroId}, Tx: {Tx}, Intentos: {Intentos})",
                        payload.SiniestroId, codigoTx, intentosTotales);

                    return new SrtNotificationOutcomeDto
                    {
                        Exitoso = true,
                        Status = "ENTREGADO_OK",
                        Mensaje = "Notificación entregada exitosamente a la SRT.",
                        Intentos = intentosTotales,
                        CodigoTransaccion = codigoTx
                    };
                }

                _logger.LogWarning("⚠[POLLY-CLIENT] Petición fallida. Estado HTTP: {StatusCode} tras {Intentos} intento(s)", response.StatusCode, intentosTotales);

                return new SrtNotificationOutcomeDto
                {
                    Exitoso = false,
                    Status = $"ERROR_HTTP_{(int)response.StatusCode}",
                    Mensaje = $"La SRT devolvió un código de error HTTP {(int)response.StatusCode}.",
                    Intentos = intentosTotales
                };
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "[POLLY-CLIENT] Petición bloqueada porque el Circuit Breaker está ABIERTO.");
                return new SrtNotificationOutcomeDto
                {
                    Exitoso = false,
                    Status = "CIRCUITO_ABIERTO",
                    Mensaje = "El circuito hacia la SRT se encuentra abierto debido a fallos reiterados.",
                    Intentos = intentosTotales > 0 ? intentosTotales : 1
                };
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogError(ex, "⏱[POLLY-CLIENT] Petición cancelada por Timeout de Polly.");
                return new SrtNotificationOutcomeDto
                {
                    Exitoso = false,
                    Status = "TIMEOUT_EXCEDIDO",
                    Mensaje = "La solicitud hacia la SRT superó el tiempo máximo de espera.",
                    Intentos = intentosTotales > 0 ? intentosTotales : 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[POLLY-CLIENT] Excepción no controlada tras agotar reintentos Polly.");
                return new SrtNotificationOutcomeDto
                {
                    Exitoso = false,
                    Status = "FALLO_REINTENTOS_EXCEDIDOS",
                    Mensaje = $"Error al comunicar con la SRT: {ex.Message}",
                    Intentos = intentosTotales > 0 ? intentosTotales : 1
                };
            }
        }
    }
}
