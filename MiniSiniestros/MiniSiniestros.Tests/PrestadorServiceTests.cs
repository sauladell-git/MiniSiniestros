using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class PrestadorServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IPrestadorRepository> _prestadorRepoMock;
        private readonly Mock<ISiniestroPrestadorRepository> _siniestroPrestadorRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PrestadorService>> _loggerMock;
        private readonly PrestadorService _service;

        public PrestadorServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _prestadorRepoMock = new Mock<IPrestadorRepository>();
            _siniestroPrestadorRepoMock = new Mock<ISiniestroPrestadorRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PrestadorService>>();

            _uowMock.Setup(u => u.Prestadores).Returns(_prestadorRepoMock.Object);
            _uowMock.Setup(u => u.SiniestroPrestadores).Returns(_siniestroPrestadorRepoMock.Object);

            _service = new PrestadorService(_uowMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_IdExistente_RetornaPrestador()
        {
            // Arrange
            var entity = new Prestador { Id = 1, Nombre = "Sanatorio Otamendi" };
            var dto = new PrestadorDto { Id = 1, Nombre = "Sanatorio Otamendi" };

            _prestadorRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<PrestadorDto>(entity)).Returns(dto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Sanatorio Otamendi", result.Data.Nombre);
        }

        [Fact]
        public async Task GetByIdAsync_IdNoExistente_RetornaFailNotFound()
        {
            // Arrange
            _prestadorRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Prestador?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.False(result.Success);
            Assert.Single(result.Errors);
            Assert.Equal("PREST_NOT_FOUND", result.Errors[0].Code);
        }

        [Fact]
        public async Task GetPrestadoresPorSiniestrosAsync_RetornaPrestadoresAsignados()
        {
            // Arrange
            var spList = new List<Siniestro_Prestador>
            {
                new Siniestro_Prestador
                {
                    SiniestroId = 10,
                    PrestadorId = 1,
                    Prestador = new Prestador { Id = 1, Nombre = "Clínica Bazterrica" }
                }
            };

            _siniestroPrestadorRepoMock
                .Setup(r => r.GetPrestadoresPorSiniestroAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(spList);

            _mapperMock
                .Setup(m => m.Map<PrestadorDto>(It.IsAny<Prestador>()))
                .Returns((Prestador p) => new PrestadorDto { Id = p.Id, Nombre = p.Nombre });

            // Act
            var result = await _service.GetPrestadoresPorSiniestrosAsync(10);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Clínica Bazterrica", result.Data[0].Nombre);
        }
    }
}
