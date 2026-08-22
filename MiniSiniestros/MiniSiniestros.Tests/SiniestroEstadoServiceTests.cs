using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using Moq;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class SiniestroEstadoServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SiniestroEstadoService>> _loggerMock;
        private readonly Mock<ISiniestroEstadoRepository> _estadoRepoMock;
        private readonly SiniestroEstadoService _service;

        public SiniestroEstadoServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SiniestroEstadoService>>();
            _estadoRepoMock = new Mock<ISiniestroEstadoRepository>();

            _uowMock.Setup(u => u.SiniestroEstados).Returns(_estadoRepoMock.Object);

            _service = new SiniestroEstadoService(
                _uowMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExisteEstadoAsync_EstadoExiste_DevuelveOkTrue()
        {
            // Arrange
            _estadoRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SiniestroEstado, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ExisteEstadoAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().BeTrue();
        }

        [Fact]
        public async Task ExisteEstadoAsync_EstadoNoExiste_DevuelveFailEstadoNoDisponible()
        {
            // Arrange
            _estadoRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SiniestroEstado, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.ExisteEstadoAsync(99);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.EstadoNoDisponible.Code);
        }

        [Fact]
        public async Task GetByIdAsync_Existente_DevuelveDto()
        {
            // Arrange
            var estado = new SiniestroEstado { Id = 1, Nombre = "Recibido" };
            var estadoDto = new SiniestroEstadoDto { Id = 1, Nombre = "Recibido" };

            _estadoRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(estado);

            _mapperMock.Setup(m => m.Map<SiniestroEstadoDto>(estado)).Returns(estadoDto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Nombre.Should().Be("Recibido");
        }

        [Fact]
        public async Task GetByIdAsync_Inexistente_DevuelveFailEstadoNoDisponible()
        {
            // Arrange
            _estadoRepoMock
                .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((SiniestroEstado?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.EstadoNoDisponible.Code);
        }

        [Fact]
        public async Task GetAllAsync_RetornaListaDeEstados()
        {
            // Arrange
            var list = new List<SiniestroEstado> { new SiniestroEstado { Id = 1, Nombre = "Recibido" } };
            var dtoList = new List<SiniestroEstadoDto> { new SiniestroEstadoDto { Id = 1, Nombre = "Recibido" } };

            _estadoRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
            _mapperMock.Setup(m => m.Map<IReadOnlyList<SiniestroEstadoDto>>(list)).Returns(dtoList);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Count.Should().Be(1);
            result.Data[0].Nombre.Should().Be("Recibido");
        }
    }
}
