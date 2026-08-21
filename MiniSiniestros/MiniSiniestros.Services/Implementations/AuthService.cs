using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MiniSiniestros.Common.Constants;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Data.UnitOfWork;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUoWData _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUoWData unitOfWork,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Intento de inicio de sesión con datos nulos o vacíos.");
                return ServiceResponse<AuthResponseDto>.Fail(SiniestroErrorConstants.CredencialesInvalidas);
            }

            _logger.LogInformation("🔑 Solicitud de autenticación iniciada para usuario: '{Nombre}'", dto.Nombre);

            var usuario = await _unitOfWork.Usuarios.GetByNombreConRolesAsync(dto.Nombre, cancellationToken);
            if (usuario == null || usuario.Password != dto.Password)
            {
                _logger.LogWarning("⚠️ Autenticación fallida: Credenciales inválidas para usuario '{Nombre}'", dto.Nombre);
                return ServiceResponse<AuthResponseDto>.Fail(SiniestroErrorConstants.CredencialesInvalidas);
            }

            var rolesList = usuario.UsuarioRoles
                .Where(ur => ur.Rol != null)
                .Select(ur => ur.Rol.Nombre)
                .ToList();

            var secretKey = _configuration["JwtSettings:Secret"] ?? "MiniSiniestrosSuperSecretKeyForJWTAuthToken2026!MustBeLongEnough";
            var issuer = _configuration["JwtSettings:Issuer"] ?? "MiniSiniestrosApi";
            var audience = _configuration["JwtSettings:Audience"] ?? "MiniSiniestrosApp";
            var expirationMinutes = int.TryParse(_configuration["JwtSettings:ExpirationInMinutes"], out var exp) ? exp : 60;

            var expiration = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.GivenName, usuario.Nombre),
                new Claim(ClaimTypes.Surname, usuario.Apellido)
            };

            foreach (var rol in rolesList)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenObject = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(tokenObject);

            _logger.LogInformation("✅ Autenticación exitosa para usuario '{Nombre}' (ID: {UsuarioId}). Roles asignados: [{Roles}]. Token JWT generado.",
                usuario.Nombre, usuario.Id, string.Join(", ", rolesList));

            var response = new AuthResponseDto
            {
                Token = tokenString,
                Expiration = expiration,
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Roles = rolesList
            };

            return ServiceResponse<AuthResponseDto>.Ok(response, "Autenticación exitosa.");
        }
    }
}
