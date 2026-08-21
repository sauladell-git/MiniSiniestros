using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// Inicia sesión y genera un Token JWT con los roles del usuario.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
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
