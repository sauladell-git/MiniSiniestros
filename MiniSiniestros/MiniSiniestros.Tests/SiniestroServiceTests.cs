using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Enums;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Empleador;
using MiniSiniestros.Dto.Prestador;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Dto.Str;
using MiniSiniestros.Dto.Trabajador;
using MiniSiniestros.Entities;
using MiniSiniestros.Services.Implementations;
using MiniSiniestros.Services.Interfaces;
using Moq;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class SiniestroServiceTests
    {
        private readonly Mock<IUoWData> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SiniestroService>> _loggerMock;
        private readonly Mock<IEmpleadorService> _empleadorServiceMock;
        private readonly Mock<ITrabajadorService> _trabajadorServiceMock;
        private readonly Mock<ISiniestroEstadoService> _siniestroEstadoServiceMock;
        private readonly Mock<IPrestadorService> _prestadorServiceMock;
        private readonly Mock<IStrNotificationService> _strNotificationServiceMock;

        private readonly Mock<ISiniestroRepository> _siniestroRepoMock;
        private readonly Mock<ISiniestroEstadoHistorialRepository> _historialRepoMock;
        private readonly Mock<ISiniestroPrestadorRepository> _siniestroPrestadorRepoMock;
        private readonly Mock<IPrestadorRepository> _prestadorRepoMock;
        private readonly Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> _transactionMock;

        private readonly SiniestroService _service;

        public SiniestroServiceTests()
        {
            _uowMock = new Mock<IUoWData>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SiniestroService>>();
            _empleadorServiceMock = new Mock<IEmpleadorService>();
            _trabajadorServiceMock = new Mock<ITrabajadorService>();
            _siniestroEstadoServiceMock = new Mock<ISiniestroEstadoService>();
            _prestadorServiceMock = new Mock<IPrestadorService>();
            _strNotificationServiceMock = new Mock<IStrNotificationService>();

            _siniestroRepoMock = new Mock<ISiniestroRepository>();
            _historialRepoMock = new Mock<ISiniestroEstadoHistorialRepository>();
            _siniestroPrestadorRepoMock = new Mock<ISiniestroPrestadorRepository>();
            _prestadorRepoMock = new Mock<IPrestadorRepository>();
            _transactionMock = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();

            _uowMock.Setup(u => u.Siniestros).Returns(_siniestroRepoMock.Object);
            _uowMock.Setup(u => u.SiniestroEstadoHistoriales).Returns(_historialRepoMock.Object);
            _uowMock.Setup(u => u.SiniestroPrestadores).Returns(_siniestroPrestadorRepoMock.Object);
            _uowMock.Setup(u => u.Prestadores).Returns(_prestadorRepoMock.Object);

            _prestadorServiceMock
                .Setup(p => p.GetPrestadoresPorSiniestrosAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<IReadOnlyList<PrestadorDto>>.Ok(new List<PrestadorDto>()));

            _strNotificationServiceMock
                .Setup(s => s.GetBySiniestroIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<IReadOnlyList<NotificacionSrtDto>>.Ok(new List<NotificacionSrtDto>()));

            _uowMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

            _service = new SiniestroService(
                _uowMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _empleadorServiceMock.Object,
                _trabajadorServiceMock.Object,
                _siniestroEstadoServiceMock.Object,
                _prestadorServiceMock.Object,
                _strNotificationServiceMock.Object);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("30-11111111-1")]
        [InlineData("3011111111A")]
        [InlineData("301111111111")]
        [InlineData("")]
        [InlineData("           ")]
        public async Task CreateAsync_ConCuitInvalidoRegex_DevuelveCuitInvalidoError(string cuitInvalido)
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = cuitInvalido, CuilTrabajador = "20111111111" };
            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.CuitInvalido.Code);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("20-11111111-1")]
        [InlineData("2011111111A")]
        [InlineData("201111111111")]
        [InlineData("")]
        [InlineData("           ")]
        public async Task CreateAsync_ConCuilInvalidoRegex_DevuelveCuilInvalidoError(string cuilInvalido)
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = "30111111111", CuilTrabajador = cuilInvalido };
            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.CuilInvalido.Code);
        }

        [Fact]
        public async Task CreateAsync_EmpleadorNoExiste_DevuelveEmpleadorNotFoundError()
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = "30999999999", CuilTrabajador = "20111111111" };

            _empleadorServiceMock
                .Setup(e => e.GetByCuitAsync("30999999999", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<EmpleadorDto>.Fail(SiniestroErrorConstants.EmpleadorNotFound));

            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.EmpleadorNotFound.Code);
        }

        [Fact]
        public async Task CreateAsync_TrabajadorNoExiste_DevuelveTrabajadorNotFoundError()
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = "30111111111", CuilTrabajador = "20999999999" };

            _empleadorServiceMock
                .Setup(e => e.GetByCuitAsync("30111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<EmpleadorDto>.Ok(new EmpleadorDto { Id = 10 }));

            _trabajadorServiceMock
                .Setup(t => t.GetByCuilAsync("20999999999", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<TrabajadorDto>.Fail(SiniestroErrorConstants.TrabajadorNotFound));

            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.TrabajadorNotFound.Code);
        }

        [Fact]
        public async Task CreateAsync_TrabajadorNoPerteneceAEmpleador_DevuelveTrabajadorNoPerteneceAEmpleadorError()
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = "30111111111", CuilTrabajador = "20111111111" };

            _empleadorServiceMock
                .Setup(e => e.GetByCuitAsync("30111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<EmpleadorDto>.Ok(new EmpleadorDto { Id = 10, Cuit = "30111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.GetByCuilAsync("20111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<TrabajadorDto>.Ok(new TrabajadorDto { Id = 5, Cuil = "20111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.ExistePorTrabajadorYEmpleadorAsync(5, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(false));

            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.TrabajadorNoPerteneceAEmpleador.Code);
        }

        [Fact]
        public async Task CreateAsync_Exitoso_AutogeneraNumeroSegunUltimoNumeroMasUno()
        {
            var dto = new CreateSiniestroDto
            {
                CuilEmpleador = "30111111111",
                CuilTrabajador = "20111111111",
                Observaciones = "Test obs"
            };

            _empleadorServiceMock
                .Setup(e => e.GetByCuitAsync("30111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<EmpleadorDto>.Ok(new EmpleadorDto { Id = 10, Cuit = "30111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.GetByCuilAsync("20111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<TrabajadorDto>.Ok(new TrabajadorDto { Id = 5, Cuil = "20111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.ExistePorTrabajadorYEmpleadorAsync(5, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            _siniestroRepoMock
                .Setup(r => r.GetUltimoNumeroAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1005);

            var createdEntity = new Siniestro { Id = 99, Numero = 1006, EmpleadorId = 10, TrabajadorId = 5, SiniestroEstadoId = 1 };

            _mapperMock.Setup(m => m.Map<Siniestro>(dto)).Returns(createdEntity);
            _siniestroRepoMock.Setup(r => r.GetByIdConDetallesAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(createdEntity);

            var createdDto = new SiniestroDto { Id = 99, Numero = 1006, Observaciones = "Test obs" };
            _mapperMock.Setup(m => m.Map<SiniestroDto>(createdEntity)).Returns(createdDto);

            _siniestroPrestadorRepoMock.Setup(r => r.GetPrestadoresPorSiniestroAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Siniestro_Prestador>());
            _historialRepoMock.Setup(r => r.GetHistorialPorSiniestroAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(new List<SiniestroEstadoHistorial>());

            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Numero.Should().Be(1006);
        }

        [Fact]
        public async Task CreateAsync_ExcepcionEnTransaccion_HaceRollbackYDevuelveSystemError()
        {
            var dto = new CreateSiniestroDto { CuilEmpleador = "30111111111", CuilTrabajador = "20111111111" };

            _empleadorServiceMock
                .Setup(e => e.GetByCuitAsync("30111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<EmpleadorDto>.Ok(new EmpleadorDto { Id = 10, Cuit = "30111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.GetByCuilAsync("20111111111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<TrabajadorDto>.Ok(new TrabajadorDto { Id = 5, Cuil = "20111111111" }));

            _trabajadorServiceMock
                .Setup(t => t.ExistePorTrabajadorYEmpleadorAsync(5, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            _mapperMock.Setup(m => m.Map<Siniestro>(dto)).Returns(new Siniestro { Id = 99 });

            _siniestroRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Siniestro>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Fallo DB simulado"));

            var result = await _service.CreateAsync(dto);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SystemError.Code);
            _uowMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_FiltroValido_DevuelvePagedResponseMapped()
        {
            var filter = new SiniestroFilterRequest { PageNumber = 1, PageSize = 10 };
            var list = new List<Siniestro> { new Siniestro { Id = 1 } };

            _siniestroRepoMock
                .Setup(r => r.GetPagedAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync((list, 1));

            _mapperMock.Setup(m => m.Map<SiniestroDto>(It.IsAny<Siniestro>())).Returns(new SiniestroDto { Id = 1 });

            var result = await _service.GetPagedAsync(filter);
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TotalRecords.Should().Be(1);
        }

        [Fact]
        public async Task GetPagedAsync_FiltroNull_DevuelvePagedResponseDefault()
        {
            var list = new List<Siniestro>();

            _siniestroRepoMock
                .Setup(r => r.GetPagedAsync(It.IsAny<SiniestroFilterRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((list, 0));

            var result = await _service.GetPagedAsync(null!);
            result.Success.Should().BeTrue();
            result.Data!.TotalRecords.Should().Be(0);
        }

        [Fact]
        public async Task GetByIdAsync_Inexistente_DevuelveFailSiniestroNotFound()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdConDetallesAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Siniestro?)null);

            var result = await _service.GetByIdAsync(999);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SiniestroNotFound.Code);
        }

        [Fact]
        public async Task GetByIdAsync_Existente_DevuelveDtoConDetalles()
        {
            var s = new Siniestro { Id = 1, Numero = 1001 };
            var sDto = new SiniestroDto { Id = 1, Numero = 1001 };

            _siniestroRepoMock.Setup(r => r.GetByIdConDetallesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(s);
            _mapperMock.Setup(m => m.Map<SiniestroDto>(s)).Returns(sDto);

            var result = await _service.GetByIdAsync(1);
            result.Success.Should().BeTrue();
            result.Data!.Id.Should().Be(1);
        }

        [Fact]
        public async Task CambiarEstadoAsync_EstadoNoExiste_DevuelveEstadoNoDisponibleError()
        {
            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Fail(SiniestroErrorConstants.EstadoNoDisponible));

            var result = await _service.CambiarEstadoAsync(1, 99);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.EstadoNoDisponible.Code);
        }

        [Fact]
        public async Task CambiarEstadoAsync_SiniestroNoExiste_DevuelveSiniestroNotFoundError()
        {
            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Siniestro?)null);

            var result = await _service.CambiarEstadoAsync(999, 2);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SiniestroNotFound.Code);
        }

        [Fact]
        public async Task CambiarEstadoAsync_Exitoso_ActualizaEstadoYGrabaHistorial()
        {
            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            var siniestro = new Siniestro { Id = 1, SiniestroEstadoId = 1 };
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(siniestro);

            var result = await _service.CambiarEstadoAsync(1, 2);
            result.Success.Should().BeTrue();
            siniestro.SiniestroEstadoId.Should().Be(2);
            _historialRepoMock.Verify(h => h.AddAsync(It.Is<SiniestroEstadoHistorial>(x => x.SiniestroId == 1 && x.SiniestroEstadoId == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CambiarEstadoAsync_AAprobado_ExcepcionAlNotificarSRT_NoBloqueaCambioDeEstado()
        {
            var aprobadoId = (int)SiniestroEstadoEnum.Aprobado;

            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(aprobadoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            var siniestro = new Siniestro { Id = 10, SiniestroEstadoId = 1 };
            _siniestroRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(siniestro);

            _strNotificationServiceMock
                .Setup(s => s.NotificarAprobacionSrtAsync(10, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Fallo SRT simulado"));

            var result = await _service.CambiarEstadoAsync(10, aprobadoId);
            result.Success.Should().BeTrue();
            siniestro.SiniestroEstadoId.Should().Be(aprobadoId);
        }

        [Fact]
        public async Task CambiarEstadoAsync_ExcepcionEnTransaccion_HaceRollbackYDevuelveSystemError()
        {
            _siniestroEstadoServiceMock
                .Setup(s => s.ExisteEstadoAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 1 });

            _historialRepoMock
                .Setup(h => h.AddAsync(It.IsAny<SiniestroEstadoHistorial>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Error DB"));

            var result = await _service.CambiarEstadoAsync(1, 2);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SystemError.Code);
            _uowMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AsignarPrestadorAsync_SiniestroNoExiste_DevuelveSiniestroNotFoundError()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Siniestro?)null);

            var result = await _service.AsignarPrestadorAsync(999, 10);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SiniestroNotFound.Code);
        }

        [Fact]
        public async Task AsignarPrestadorAsync_PrestadorNoExiste_DevuelvePrestadorNotFoundError()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 1 });

            _prestadorServiceMock
                .Setup(p => p.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<PrestadorDto>.Fail(SiniestroErrorConstants.PrestadorNotFound));

            var result = await _service.AsignarPrestadorAsync(1, 999);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.PrestadorNotFound.Code);
        }

        [Fact]
        public async Task AsignarPrestadorAsync_PrestadorYaAsignado_DevuelvePrestadorYaAsignadoError()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 1 });

            _prestadorServiceMock
                .Setup(p => p.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<PrestadorDto>.Ok(new PrestadorDto { Id = 10 }));

            _siniestroPrestadorRepoMock
                .Setup(sp => sp.ExistsAsync(It.IsAny<Expression<Func<Siniestro_Prestador, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _service.AsignarPrestadorAsync(1, 10);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.PrestadorYaAsignado.Code);
        }

        [Fact]
        public async Task AsignarPrestadorAsync_Exitoso_GuardaRelacionSiniestroPrestador()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 1 });

            _prestadorServiceMock
                .Setup(p => p.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<PrestadorDto>.Ok(new PrestadorDto { Id = 10 }));

            _siniestroPrestadorRepoMock
                .Setup(sp => sp.ExistsAsync(It.IsAny<Expression<Func<Siniestro_Prestador, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _service.AsignarPrestadorAsync(1, 10);
            result.Success.Should().BeTrue();
            _siniestroPrestadorRepoMock.Verify(sp => sp.AddAsync(It.Is<Siniestro_Prestador>(x => x.SiniestroId == 1 && x.PrestadorId == 10), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AsignarPrestadorAsync_ExcepcionEnTransaccion_HaceRollbackYDevuelveSystemError()
        {
            _siniestroRepoMock
                .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Siniestro { Id = 1 });

            _prestadorServiceMock
                .Setup(p => p.GetByIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<PrestadorDto>.Ok(new PrestadorDto { Id = 10 }));

            _siniestroPrestadorRepoMock
                .Setup(sp => sp.ExistsAsync(It.IsAny<Expression<Func<Siniestro_Prestador, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _siniestroPrestadorRepoMock
                .Setup(sp => sp.AddAsync(It.IsAny<Siniestro_Prestador>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("DB Error"));

            var result = await _service.AsignarPrestadorAsync(1, 10);
            result.Success.Should().BeFalse();
            result.Errors.First().Code.Should().Be(SiniestroErrorConstants.SystemError.Code);
            _uowMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
