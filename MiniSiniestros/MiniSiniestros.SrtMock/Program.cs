using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Dto.Str;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapPost("/srt-mock/notificar", async (
    [FromBody] SrtPayloadDto payload,
    [FromQuery] bool? simularFallo,
    ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("SrtMock");

    if (payload == null || payload.SiniestroId <= 0)
    {
        logger.LogWarning("[SRT-MOCK] Petición recibida con payload nulo o SiniestroId inválido.");
        return Results.BadRequest(new { Recibido = false, Mensaje = "SiniestroId e información de payload son requeridos." });
    }

    // Falla con un 33% de probabilidad (1 de cada 3 intentos) o forzado por query string (?simularFallo=true)
    bool debeFallar = simularFallo ?? (Random.Shared.Next(1, 4) == 1);

    if (debeFallar)
    {
        logger.LogWarning("[SRT-MOCK] Simulación de error 500 activada para SiniestroId: {SiniestroId}", payload.SiniestroId);
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }

    var transactionCode = $"SRT-TX-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    logger.LogInformation("[SRT-MOCK] Notificación procesada con éxito para SiniestroId: {SiniestroId}. Transacción: {Tx}", payload.SiniestroId, transactionCode);

    await Task.Delay(100); // Pequeña latencia simulada de red

    return Results.Ok(new
    {
        Recibido = true,
        CodigoTransaccion = transactionCode,
        Mensaje = "Notificación recibida en SRT simulada",
        Timestamp = DateTime.UtcNow
    });
});

app.Run();
