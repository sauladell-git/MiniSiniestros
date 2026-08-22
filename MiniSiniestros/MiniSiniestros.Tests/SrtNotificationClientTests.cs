using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class SrtNotificationClientTests
    {
        private readonly Mock<ILogger<SrtNotificationClient>> _loggerMock;

        public SrtNotificationClientTests()
        {
            _loggerMock = new Mock<ILogger<SrtNotificationClient>>();
        }

        [Fact]
        public async Task NotificarAprobacionAsync_Http200Ok_RetornaStatusEntregadoOk()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var responseMsg = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { success = true, codigoTransaccion = "TX-9999", message = "Notificación SRT recibida" })
            };

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMsg);

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8082/")
            };

            var client = new SrtNotificationClient(httpClient, _loggerMock.Object);
            var payload = new SrtPayloadDto { SiniestroId = 10, Estado = "Aprobado", FechaAprobacion = DateTime.UtcNow };

            // Act
            var outcome = await client.NotificarAprobacionAsync(payload);

            // Assert
            Assert.NotNull(outcome);
            Assert.True(outcome.Exitoso);
            Assert.Equal("ENTREGADO_OK", outcome.Status);
            Assert.Equal("TX-9999", outcome.CodigoTransaccion);
            Assert.Equal(1, outcome.Intentos);
        }

        [Fact]
        public async Task NotificarAprobacionAsync_HttpError_ManejaFallosConPolly()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Servicio SRT Caído"));

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8082/")
            };

            var client = new SrtNotificationClient(httpClient, _loggerMock.Object);
            var payload = new SrtPayloadDto { SiniestroId = 10, Estado = "Aprobado", FechaAprobacion = DateTime.UtcNow };

            // Act
            var outcome = await client.NotificarAprobacionAsync(payload);

            // Assert
            Assert.NotNull(outcome);
            Assert.False(outcome.Exitoso);
            Assert.Contains(outcome.Status, new[] { "CIRCUITO_ABIERTO", "FALLO_REINTENTOS_EXCEDIDOS" });
            Assert.True(outcome.Intentos >= 1);
        }
    }
}
