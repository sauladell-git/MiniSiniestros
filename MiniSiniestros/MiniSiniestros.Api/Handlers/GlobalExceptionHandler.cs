using Microsoft.AspNetCore.Diagnostics;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;

namespace MiniSiniestros.Api.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Excepción no controlada capturada en el pipeline de la API. Ruta: {Path}, Método HTTP: {Method}",
                httpContext.Request.Path,
                httpContext.Request.Method);

            var response = ServiceResponse<object>.Fail(
                SiniestroErrorConstants.SystemError,
                "Ocurrió un error inesperado al procesar la solicitud en el servidor.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

            return true;
        }
    }
}
