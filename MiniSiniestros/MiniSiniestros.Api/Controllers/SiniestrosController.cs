using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión completa de Siniestros laborales.
    /// Requiere autenticación JWT Bearer en todas sus operaciones.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SiniestrosController : ControllerBase
    {
        private readonly ISiniestroService _siniestroService;

        public SiniestrosController(ISiniestroService siniestroService)
        {
            _siniestroService = siniestroService ?? throw new ArgumentNullException(nameof(siniestroService));
        }

        /// <summary>
        /// Obtiene el listado paginado y filtrado de siniestros laborales.
        /// </summary>
        /// <remarks>
        /// Permite buscar siniestros aplicando múltiples criterios combinables:
        /// - **cuit**: CUIT del Empleador (11 dígitos).
        /// - **cuil**: CUIL del Trabajador (11 dígitos).
        /// - **fechaDesde** / **fechaHasta**: Rango de fechas de siniestro.
        /// - **siniestroEstadoId**: ID del estado (1: Recibido, 2: EnProceso, 3: Aprobado, 4: Rechazado, 5: Finalizado).
        /// - **pageNumber** &amp; **pageSize**: Parámetros de paginación (por defecto página 1, tamaño 10).
        /// - **sortBy**: Campo para ordenamiento (`fecha`, `numero`, `empleador`, `trabajador`, `estado`).
        /// - **isDescending**: `true` para orden descendente, `false` para ascendente.
        /// </remarks>
        /// <param name="filter">Parámetros de filtrado, ordenamiento y paginación en la query string.</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>Estructura `PagedResponse` envuelta en `ServiceResponse` con el listado paginado de siniestros.</returns>
        /// <response code="200">Devuelve el listado paginado de siniestros procesado correctamente.</response>
        /// <response code="401">No autorizado. Falta el Token JWT en el encabezado `Authorization` o ha expirado.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResponse<PagedResponse<SiniestroDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceResponse<PagedResponse<SiniestroDto>>>> GetPaged([FromQuery] SiniestroFilterRequest filter, CancellationToken cancellationToken)
        {
            var response = await _siniestroService.GetPagedAsync(filter, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene los detalles completos de un siniestro por su ID.
        /// </summary>
        /// <remarks>
        /// Retorna la entidad del siniestro junto a sus relaciones:
        /// - Datos del Empleador y Trabajador asociado.
        /// - Prestadores médicos asignados.
        /// - Historial cronológico de cambios de estado.
        /// - Registro de notificaciones enviadas a la SRT.
        /// </remarks>
        /// <param name="id">ID numérico único del siniestro.</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>DTO detallado del siniestro.</returns>
        /// <response code="200">Siniestro encontrado. Retorna la información completa.</response>
        /// <response code="404">No encontrado. El siniestro con el ID especificado no existe.</response>
        /// <response code="401">No autorizado. El Token JWT es inválido o no fue provisto.</response>
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
        /// Registra un nuevo siniestro laboral en la plataforma.
        /// </summary>
        /// <remarks>
        /// **Reglas de Validación y Negocio:**
        /// - **CuitEmpleador** y **CuilTrabajador**: Deben ser estrictamente 11 dígitos numéricos sin guiones.
        /// - Verifica existencia de Empleador y Trabajador en el sistema.
        /// - Verifica que el Trabajador pertenezca al Empleador indicado.
        /// - Asigna automáticamente el estado inicial **Recibido (1)** y genera el número consecutivo.
        /// 
        /// **Política de Rol Requerida:** `RequireOperadorRole` (Permitido para `Administrador` u `Operador`).
        /// </remarks>
        /// <param name="dto">Datos del nuevo siniestro (`CuitEmpleador`, `CuilTrabajador`, `Observaciones`).</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>Objeto `SiniestroDto` recién creado con su ID y número asignado.</returns>
        /// <response code="201">Siniestro creado exitosamente. Retorna la entidad creada y la cabecera `Location`.</response>
        /// <response code="400">Error de validación de datos (CUIT/CUIL inválido, relación inexistente, etc.).</response>
        /// <response code="401">No autorizado. Requiere estar autenticado con Token JWT.</response>
        /// <response code="403">Prohibido. El usuario autenticado (ej: `Analista`) no tiene permisos de creación.</response>
        [HttpPost]
        [Authorize(Policy = "RequireOperadorRole")]
        [Consumes("application/json")]
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
        /// Modifica el estado actual de un siniestro.
        /// </summary>
        /// <remarks>
        /// Actualiza el estado del siniestro y registra la transición en el historial con fecha y hora UTC.
        /// Si el estado cambia a **Aprobado (3)**, desencadena la notificación automática ante la SRT.
        /// 
        /// **Estados Disponibles:**
        /// - `1`: Recibido
        /// - `2`: EnProceso
        /// - `3`: Aprobado
        /// - `4`: Rechazado
        /// - `5`: Finalizado
        /// 
        /// **Política de Rol Requerida:** `RequireOperadorRole` (Permitido para `Administrador` u `Operador`).
        /// </remarks>
        /// <param name="id">ID del siniestro a modificar.</param>
        /// <param name="dto">DTO con el `NuevoEstadoId` deseado.</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>Resultado booleano indicando el éxito del cambio de estado.</returns>
        /// <response code="200">Estado modificado e historial registrado correctamente.</response>
        /// <response code="400">Estado no válido o siniestro inexistente.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Prohibido. El rol del usuario no tiene permisos para cambiar estados.</response>
        [HttpPatch("{id:int}/estado")]
        [Authorize(Policy = "RequireOperadorRole")]
        [Consumes("application/json")]
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
        /// Asigna un prestador médico asistencial a un siniestro.
        /// </summary>
        /// <remarks>
        /// Registra la vinculación entre un siniestro y un prestador médico para la atención del trabajador.
        /// 
        /// **Política de Rol Requerida:** `RequireOperadorRole` (Permitido para `Administrador` u `Operador`).
        /// </remarks>
        /// <param name="id">ID del siniestro.</param>
        /// <param name="dto">DTO con el `PrestadorId` a vincular.</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>Resultado booleano de la asignación.</returns>
        /// <response code="200">Prestador asignado al siniestro correctamente.</response>
        /// <response code="400">Prestador o Siniestro inexistente.</response>
        /// <response code="401">No autorizado.</response>
        /// <response code="403">Prohibido. El rol del usuario no tiene permisos para asignar prestadores.</response>
        [HttpPost("{id:int}/prestadores")]
        [Authorize(Policy = "RequireOperadorRole")]
        [Consumes("application/json")]
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
