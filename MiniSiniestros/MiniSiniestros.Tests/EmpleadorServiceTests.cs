using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class EmpleadorServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IEmpleadorRepository> _empleadorRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EmpleadorService>> _loggerMock;
        private readonly EmpleadorService _service;

        public EmpleadorServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _empleadorRepoMock = new Mock<IEmpleadorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EmpleadorService>>();

            _uowMock.Setup(u => u.Empleadores).Returns(_empleadorRepoMock.Object);
            _service = new EmpleadorService(_uowMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_IdExistente_RetornaEmpleador()
        {
            // Arrange
            var entity = new Empleador { Id = 1, RazonSocial = "Tech SA", Cuit = "30111111111" };
            var dto = new EmpleadorDto { Id = 1, RazonSocial = "Tech SA", Cuit = "30111111111" };

            _empleadorRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<EmpleadorDto>(entity)).Returns(dto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Tech SA", result.Data.RazonSocial);
        }

        [Fact]
        public async Task GetByIdAsync_IdInexistente_RetornaFailNotFound()
        {
            // Arrange
            _empleadorRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Empleador?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("EMP_NOT_FOUND", result.Errors[0].Code);
        }

        [Fact]
        public async Task GetByCuitAsync_CuitValido_RetornaEmpleador()
        {
            // Arrange
            var entity = new Empleador { Id = 1, RazonSocial = "Tech SA", Cuit = "30111111111" };
            var dto = new EmpleadorDto { Id = 1, RazonSocial = "Tech SA", Cuit = "30111111111" };

            _empleadorRepoMock.Setup(r => r.GetByCuitAsync("30111111111", It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<EmpleadorDto>(entity)).Returns(dto);

            // Act
            var result = await _service.GetByCuitAsync("30111111111");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("30111111111", result.Data.Cuit);
        }

        [Fact]
        public async Task GetByCuitAsync_CuitInexistente_RetornaFailNotFound()
        {
            // Arrange
            _empleadorRepoMock.Setup(r => r.GetByCuitAsync("30999999999", It.IsAny<CancellationToken>())).ReturnsAsync((Empleador?)null);

            // Act
            var result = await _service.GetByCuitAsync("30999999999");

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("EMP_NOT_FOUND", result.Errors[0].Code);
        }
    }
}
