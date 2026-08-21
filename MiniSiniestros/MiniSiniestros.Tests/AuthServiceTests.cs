using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);

            _configMock.Setup(c => c["JwtSettings:Secret"]).Returns("MiniSiniestrosSuperSecretKeyForJWTAuthToken2026!MustBeLongEnough");
            _configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("MiniSiniestrosApi");
            _configMock.Setup(c => c["JwtSettings:Audience"]).Returns("MiniSiniestrosApp");
            _configMock.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("60");

            _service = new AuthService(_uowMock.Object, _configMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task LoginAsync_CredencialesValidas_RetornaTokenYRoles()
        {
            // Arrange
            var user = new Usuario
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                Password = "AdminPassword*2026",
                UsuarioRoles = new List<Usuario_Rol>
                {
                    new Usuario_Rol { RolId = 1, Rol = new Rol { Id = 1, Nombre = "Administrador" } }
                }
            };

            _usuarioRepoMock
                .Setup(r => r.GetByNombreConRolesAsync("Juan", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var dto = new LoginDto
            {
                Nombre = "Juan",
                Password = "AdminPassword*2026"
            };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.False(string.IsNullOrWhiteSpace(result.Data.Token));
            Assert.Equal("Juan", result.Data.Nombre);
            Assert.Single(result.Data.Roles);
            Assert.Contains("Administrador", result.Data.Roles);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Data.Token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
            Assert.NotNull(roleClaim);
            Assert.Equal("Administrador", roleClaim.Value);
        }

        [Fact]
        public async Task LoginAsync_PasswordIncorrecto_RetornaFail()
        {
            // Arrange
            var user = new Usuario
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                Password = "AdminPassword*2026"
            };

            _usuarioRepoMock
                .Setup(r => r.GetByNombreConRolesAsync("Juan", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var dto = new LoginDto
            {
                Nombre = "Juan",
                Password = "PasswordEquivocado"
            };

            // Act
            var result = await _service.LoginAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("CREDENCIALES_INVALIDAS", result.Errors[0].Code);
        }
    }
}
