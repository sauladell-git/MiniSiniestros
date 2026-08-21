using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SiniestrosController : ControllerBase
    {
        private readonly ISiniestroService _siniestroService;

        public SiniestrosController(ISiniestroService siniestroService)
        {
            _siniestroService = siniestroService ?? throw new ArgumentNullException(nameof(siniestroService));
        }

        /// <summary>
        /// Listar siniestros con paginación, ordenamiento y filtros (estado, desde, hasta, cuit, cuil, page, pageSize)
        /// Requiere estar autenticado con JWT.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<PagedResponse<SiniestroDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceResponse<PagedResponse<SiniestroDto>>>> GetPaged([FromQuery] SiniestroFilterRequest filter, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.GetPagedAsync(filter, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Obtener detalle de siniestro con prestadores asignados e historial de estados.
        /// Requiere estar autenticado con JWT.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ServiceResponse<SiniestroDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponse<SiniestroDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceResponse<SiniestroDto>>> GetById(int id, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.GetByIdAsync(id, cancellationToken);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Crear siniestro (valida CUIT/CUIL y reglas de negocio).
        /// Requiere Rol de Operador o superior (Administrador/Operador).
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "RequireOperadorRole")]
        [ProducesResponseType(typeof(ServiceResponse<SiniestroDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ServiceResponse<SiniestroDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ServiceResponse<SiniestroDto>>> Create([FromBody] CreateSiniestroDto dto, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.CreateAsync(dto, cancellationToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        /// <summary>
        /// Cambiar estado de un siniestro con registro en historial.
        /// Requiere Rol de Operador o superior (Administrador/Operador).
        /// </summary>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Policy = "RequireOperadorRole")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ServiceResponse<bool>>> CambiarEstado(int id, [FromBody] CambiarEstadoSiniestroDto dto, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.CambiarEstadoAsync(id, dto.NuevoEstadoId, cancellationToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Asignar prestador médico a un siniestro.
        /// Requiere Rol de Operador o superior (Administrador/Operador).
        /// </summary>
        [HttpPost("{id:int}/prestadores")]
        [Authorize(Policy = "RequireOperadorRole")]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ServiceResponse<bool>>> AsignarPrestador(int id, [FromBody] AsignarPrestadorSiniestroDto dto, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.AsignarPrestadorAsync(id, dto.PrestadorId, cancellationToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
