using Microsoft.Extensions.Logging;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUoWData _unitOfWork;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(
            IUoWData unitOfWork,
            ILogger<UsuarioService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<UsuarioDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Consultando usuario por ID {UsuarioId}", id);

            var usuario = await _unitOfWork.Usuarios.GetByIdConRolesAsync(id, cancellationToken);
            if (usuario == null)
            {
                _logger.LogWarning("Usuario con ID {UsuarioId} no fue encontrado.", id);
                return ServiceResponse<UsuarioDto>.Fail(SiniestroErrorConstants.UsuarioNotFound);
            }

            var rolesList = usuario.UsuarioRoles
                .Where(ur => ur.Rol != null)
                .Select(ur => ur.Rol.Nombre)
                .ToList();

            var dto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Roles = rolesList
            };

            return ServiceResponse<UsuarioDto>.Ok(dto);
        }
    }
}
