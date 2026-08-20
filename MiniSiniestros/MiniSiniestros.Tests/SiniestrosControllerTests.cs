using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Api.Controllers;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Services.Interfaces;
using Moq;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class SiniestrosControllerTests
    {
        private readonly Mock<ISiniestroService> _serviceMock;
        private readonly SiniestrosController _controller;

        public SiniestrosControllerTests()
        {
            _serviceMock = new Mock<ISiniestroService>();
            _controller = new SiniestrosController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetPaged_Devuelve200OKConRespuestaPaginada()
        {
            // Arrange
            var filter = new SiniestroFilterRequest { PageNumber = 1, PageSize = 10 };
            var pagedData = new PagedResponse<SiniestroDto>(new List<SiniestroDto>(), 1, 10, 0);

            _serviceMock
                .Setup(s => s.GetPagedAsync(filter, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<PagedResponse<SiniestroDto>>.Ok(pagedData));

            // Act
            var actionResult = await _controller.GetPaged(filter, CancellationToken.None);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ServiceResponse<PagedResponse<SiniestroDto>>>().Subject;
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_Existente_Devuelve200OK()
        {
            // Arrange
            var siniestroDto = new SiniestroDto { Id = 1, Numero = 1001 };
            _serviceMock
                .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<SiniestroDto>.Ok(siniestroDto));

            // Act
            var actionResult = await _controller.GetById(1, CancellationToken.None);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ServiceResponse<SiniestroDto>>().Subject;
            response.Success.Should().BeTrue();
            response.Data!.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_Inexistente_Devuelve404NotFound()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.SiniestroNotFound));

            // Act
            var actionResult = await _controller.GetById(999, CancellationToken.None);

            // Assert
            var notFoundResult = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            var response = notFoundResult.Value.Should().BeOfType<ServiceResponse<SiniestroDto>>().Subject;
            response.Success.Should().BeFalse();
            response.Errors.First().Code.Should().Be(SiniestroErrorConstants.SiniestroNotFound.Code);
        }

        [Fact]
        public async Task Create_Valido_Devuelve201CreatedAtAction()
        {
            // Arrange
            var createDto = new CreateSiniestroDto { CuilEmpleador = "30111111111", CuilTrabajador = "20111111111" };
            var createdDto = new SiniestroDto { Id = 5, Numero = 1001 };

            _serviceMock
                .Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<SiniestroDto>.Ok(createdDto));

            // Act
            var actionResult = await _controller.Create(createDto, CancellationToken.None);

            // Assert
            var createdResult = actionResult.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(SiniestrosController.GetById));
            var response = createdResult.Value.Should().BeOfType<ServiceResponse<SiniestroDto>>().Subject;
            response.Success.Should().BeTrue();
            response.Data!.Id.Should().Be(5);
        }

        [Fact]
        public async Task Create_Invalido_Devuelve400BadRequest()
        {
            // Arrange
            var createDto = new CreateSiniestroDto { CuilEmpleador = "12345" };

            _serviceMock
                .Setup(s => s.CreateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<SiniestroDto>.Fail(SiniestroErrorConstants.CuitInvalido));

            // Act
            var actionResult = await _controller.Create(createDto, CancellationToken.None);

            // Assert
            var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ServiceResponse<SiniestroDto>>().Subject;
            response.Success.Should().BeFalse();
            response.Errors.First().Code.Should().Be(SiniestroErrorConstants.CuitInvalido.Code);
        }

        [Fact]
        public async Task CambiarEstado_Valido_Devuelve200OK()
        {
            // Arrange
            var dto = new CambiarEstadoSiniestroDto { NuevoEstadoId = 2 };
            _serviceMock
                .Setup(s => s.CambiarEstadoAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            // Act
            var actionResult = await _controller.CambiarEstado(1, dto, CancellationToken.None);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ServiceResponse<bool>>().Subject;
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async Task CambiarEstado_Invalido_Devuelve400BadRequest()
        {
            // Arrange
            var dto = new CambiarEstadoSiniestroDto { NuevoEstadoId = 99 };
            _serviceMock
                .Setup(s => s.CambiarEstadoAsync(1, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Fail(SiniestroErrorConstants.EstadoNoDisponible));

            // Act
            var actionResult = await _controller.CambiarEstado(1, dto, CancellationToken.None);

            // Assert
            var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ServiceResponse<bool>>().Subject;
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task AsignarPrestador_Valido_Devuelve200OK()
        {
            // Arrange
            var dto = new AsignarPrestadorSiniestroDto { PrestadorId = 10 };
            _serviceMock
                .Setup(s => s.AsignarPrestadorAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            // Act
            var actionResult = await _controller.AsignarPrestador(1, dto, CancellationToken.None);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ServiceResponse<bool>>().Subject;
            response.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AsignarPrestador_Invalido_Devuelve400BadRequest()
        {
            // Arrange
            var dto = new AsignarPrestadorSiniestroDto { PrestadorId = 10 };
            _serviceMock
                .Setup(s => s.AsignarPrestadorAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ServiceResponse<bool>.Fail(SiniestroErrorConstants.PrestadorYaAsignado));

            // Act
            var actionResult = await _controller.AsignarPrestador(1, dto, CancellationToken.None);

            // Assert
            var badRequestResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value.Should().BeOfType<ServiceResponse<bool>>().Subject;
            response.Success.Should().BeFalse();
            response.Errors.First().Code.Should().Be(SiniestroErrorConstants.PrestadorYaAsignado.Code);
        }
    }
}
