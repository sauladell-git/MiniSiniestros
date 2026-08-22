using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Api.Controllers
{
    /// <summary>
    /// Controlador para la autenticación de usuarios y emisión de Tokens JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Inicia sesión en la plataforma y obtiene un Token JWT de autenticación.
        /// </summary>
        /// <remarks>
        /// Permite a un usuario autenticarse enviando su nombre de usuario y contraseña.
        /// Retorna un token JWT firmado mediante HMAC-SHA256 con los claims de identidad y roles asignados.
        /// 
        /// **Usuarios de Prueba Disponibles:**
        /// - `Admin` / `Admin*2026` (Rol: Administrador - Acceso Total)
        /// - `Operador` / `Operador*2026` (Rol: Operador - Permisos Operativos)
        /// - `Analista` / `Analista*2026` (Rol: Analista - Acceso de Lectura)
        /// </remarks>
        /// <param name="dto">DTO con las credenciales de ingreso (`Nombre` y `Password`).</param>
        /// <param name="cancellationToken">Token de cancelación de la solicitud.</param>
        /// <returns>Objeto `ServiceResponse` que contiene el token JWT generado, datos del usuario y fecha de expiración.</returns>
        /// <response code="200">Autenticación exitosa. Retorna el token JWT y los datos del usuario.</response>
        /// <response code="400">Datos de entrada inválidos o faltantes.</response>
        /// <response code="401">Credenciales inválidas (usuario inexistente o contraseña errónea).</response>
        [AllowAnonymous]
        [HttpPost("login")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ServiceResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitud de Login recibida en la API para usuario '{Nombre}'", dto?.Nombre);
            var response = await _authService.LoginAsync(dto!, cancellationToken);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
