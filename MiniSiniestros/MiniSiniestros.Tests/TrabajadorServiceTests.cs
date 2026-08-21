using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Trabajador;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class TrabajadorServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<ITrabajadorRepository> _trabajadorRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TrabajadorService>> _loggerMock;
        private readonly TrabajadorService _service;

        public TrabajadorServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _trabajadorRepoMock = new Mock<ITrabajadorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TrabajadorService>>();

            _uowMock.Setup(u => u.Trabajadores).Returns(_trabajadorRepoMock.Object);
            _service = new TrabajadorService(_uowMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_IdExistente_RetornaTrabajador()
        {
            // Arrange
            var entity = new Trabajador { Id = 1, Nombre = "Charly", Apellido = "García", Cuil = "20111111111" };
            var list = new List<Trabajador> { entity };
            var dto = new TrabajadorDto { Id = 1, Nombre = "Charly", Apellido = "García", Cuil = "20111111111" };

            _trabajadorRepoMock.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Trabajador, bool>>>(),
                It.IsAny<Func<IQueryable<Trabajador>, IOrderedQueryable<Trabajador>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            _mapperMock.Setup(m => m.Map<TrabajadorDto>(entity)).Returns(dto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Charly", result.Data.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_IdInexistente_RetornaFailNotFound()
        {
            // Arrange
            _trabajadorRepoMock.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Trabajador, bool>>>(),
                It.IsAny<Func<IQueryable<Trabajador>, IOrderedQueryable<Trabajador>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Trabajador>());

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("TRAB_NOT_FOUND", result.Errors[0].Code);
        }

        [Fact]
        public async Task GetByCuilAsync_CuilValido_RetornaTrabajador()
        {
            // Arrange
            var entity = new Trabajador { Id = 1, Nombre = "Gustavo", Apellido = "Cerati", Cuil = "20222222222" };
            var dto = new TrabajadorDto { Id = 1, Nombre = "Gustavo", Apellido = "Cerati", Cuil = "20222222222" };

            _trabajadorRepoMock.Setup(r => r.GetByCuilAsync("20222222222", It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<TrabajadorDto>(entity)).Returns(dto);

            // Act
            var result = await _service.GetByCuilAsync("20222222222");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("20222222222", result.Data.Cuil);
        }

        [Fact]
        public async Task GetByCuilAsync_CuilInexistente_RetornaFailNotFound()
        {
            // Arrange
            _trabajadorRepoMock.Setup(r => r.GetByCuilAsync("20999999999", It.IsAny<CancellationToken>())).ReturnsAsync((Trabajador?)null);

            // Act
            var result = await _service.GetByCuilAsync("20999999999");

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("TRAB_NOT_FOUND", result.Errors[0].Code);
        }

        [Fact]
        public async Task ExistePorTrabajadorYEmpleadorAsync_RetornaTrue()
        {
            // Arrange
            _trabajadorRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Trabajador, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ExistePorTrabajadorYEmpleadorAsync(1, 2);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
        }
    }
}
