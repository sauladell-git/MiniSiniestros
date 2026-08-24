using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
        private readonly Mock<ILogger<UsuarioService>> _loggerMock;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _usuarioRepoMock = new Mock<IUsuarioRepository>();
            _loggerMock = new Mock<ILogger<UsuarioService>>();

            _uowMock.Setup(u => u.Usuarios).Returns(_usuarioRepoMock.Object);
            _service = new UsuarioService(_uowMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_UsuarioExistente_RetornaUsuarioDto()
        {
            // Arrange
            var user = new Usuario
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "Sistema",
                UsuarioRoles = new List<Usuario_Rol>
                {
                    new Usuario_Rol { RolId = 1, Rol = new Rol { Id = 1, Nombre = "Administrador" } }
                }
            };

            _usuarioRepoMock
                .Setup(r => r.GetByIdConRolesAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Admin", result.Data.Nombre);
            Assert.Equal("Sistema", result.Data.Apellido);
            Assert.Single(result.Data.Roles);
            Assert.Contains("Administrador", result.Data.Roles);
        }

        [Fact]
        public async Task GetByIdAsync_UsuarioInexistente_RetornaFailUsuarioNotFound()
        {
            // Arrange
            _usuarioRepoMock
                .Setup(r => r.GetByIdConRolesAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("USUARIO_NOT_FOUND", result.Errors[0].Code);
        }
    }
}
