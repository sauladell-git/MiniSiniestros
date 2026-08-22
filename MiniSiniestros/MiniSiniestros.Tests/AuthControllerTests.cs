using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Api.Controllers;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Services.Interfaces;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Login_CredencialesValidas_RetornaOkConToken()
        {
            // Arrange
            var dto = new LoginDto { Nombre = "Admin", Password = "Admin*2026" };
            var authResponse = new AuthResponseDto
            {
                Token = "fake-jwt-token-12345",
                UsuarioId = 1,
                Nombre = "Admin",
                Apellido = "Sistema",
                Roles = new List<string> { "Administrador" }
            };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<AuthResponseDto>.Ok(authResponse));

            // Act
            var result = await _controller.Login(dto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<AuthResponseDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("fake-jwt-token-12345", response.Data!.Token);
        }

        [Fact]
        public async Task Login_CredencialesInvalidas_RetornaUnauthorized()
        {
            // Arrange
            var dto = new LoginDto { Nombre = "Admin", Password = "WrongPassword" };

            _authServiceMock
                .Setup(s => s.LoginAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<AuthResponseDto>.Fail(SiniestroErrorConstants.CredencialesInvalidas));

            // Act
            var result = await _controller.Login(dto, CancellationToken.None);

            // Assert
            var unauthResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<AuthResponseDto>>(unauthResult.Value);
            Assert.False(response.Success);
            Assert.Equal("CREDENCIALES_INVALIDAS", response.Errors[0].Code);
        }
    }
}
