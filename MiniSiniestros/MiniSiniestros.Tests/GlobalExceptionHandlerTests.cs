using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Api.Handlers;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using Moq;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private readonly GlobalExceptionHandler _handler;

        public GlobalExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _handler = new GlobalExceptionHandler(_loggerMock.Object);
        }

        [Fact]
        public async Task TryHandleAsync_CapturaExcepcion_DevuelveStatusCode500YResponseFormateado()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            context.Request.Path = "/api/siniestros";
            context.Request.Method = "GET";

            var exception = new InvalidOperationException("Excepción de prueba no controlada.");

            // Act
            var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            context.Response.ContentType.Should().StartWith("application/json");

            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            var responseBody = await reader.ReadToEndAsync();

            var responseDto = JsonSerializer.Deserialize<ServiceResponse<object>>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            responseDto.Should().NotBeNull();
            responseDto!.Success.Should().BeFalse();
            responseDto.Errors.Should().NotBeEmpty();
            responseDto.Errors.First().Code.Should().Be(SiniestroErrorConstants.SystemError.Code);
        }
    }
}
