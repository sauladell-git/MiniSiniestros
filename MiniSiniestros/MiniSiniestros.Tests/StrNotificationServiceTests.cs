using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using MiniSiniestros.Services.Interfaces;
using MiniSiniestros.Services.Profiles;
using Moq;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class StrNotificationServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<ILogger<StrNotificationService>> _loggerMock;
        private readonly Mock<ISiniestroRepository> _siniestroRepoMock;
        private readonly Mock<INotificacionSRTRepository> _notificacionRepoMock;
        private readonly Mock<ISrtNotificationClient> _srtClientMock;
        private readonly IMapper _mapper;
        private readonly StrNotificationService _service;

        public StrNotificationServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _loggerMock = new Mock<ILogger<StrNotificationService>>();
            _siniestroRepoMock = new Mock<ISiniestroRepository>();
            _notificacionRepoMock = new Mock<INotificacionSRTRepository>();
            _srtClientMock = new Mock<ISrtNotificationClient>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());
            _mapper = mapperConfig.CreateMapper();

            _uowMock.Setup(u => u.Siniestros).Returns(_siniestroRepoMock.Object);
            _uowMock.Setup(u => u.NotificacionesSRT).Returns(_notificacionRepoMock.Object);

            _service = new StrNotificationService(_uowMock.Object, _mapper, _loggerMock.Object, _srtClientMock.Object);
        }

        [Fact]
        public async Task NotificarAprobacionSrtAsync_SiniestroValido_DisparaClientePollyYPersisteDB()
        {
            // Arrange
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 10, SiniestroEstadoId = 3 });

            _srtClientMock
                .Setup(c => c.NotificarAprobacionAsync(It.IsAny<SrtPayloadDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SrtNotificationOutcomeDto
                {
                    Exitoso = true,
                    Status = "ENTREGADO_OK",
                    Intentos = 1,
                    CodigoTransaccion = "SRT-TX-999"
                });

            // Act
            var result = await _service.NotificarAprobacionSrtAsync(10);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.SiniestroId.Should().Be(10);
            result.Data.Status.Should().Be("ENTREGADO_OK");

            _srtClientMock.Verify(c => c.NotificarAprobacionAsync(It.Is<SrtPayloadDto>(p => p.SiniestroId == 10), It.IsAny<CancellationToken>()), Times.Once);
            _notificacionRepoMock.Verify(r => r.AddAsync(It.Is<NotificacionSRT>(n => n.SiniestroId == 10 && n.Status == "ENTREGADO_OK"), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.CompleteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetBySiniestroIdAsync_DevuelveListaNotificaciones()
        {
            // Arrange
            var list = new List<NotificacionSRT>
            {
                new NotificacionSRT { Id = 1, SiniestroId = 5, Status = "ENTREGADO_OK", Timestamp = DateTime.UtcNow, Payload = "{}" }
            };

            _notificacionRepoMock
                .Setup(r => r.GetAsync(It.IsAny<Expression<Func<NotificacionSRT, bool>>>(), null, null, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            // Act
            var result = await _service.GetBySiniestroIdAsync(5);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data!.First().SiniestroId.Should().Be(5);
        }
    }
}
