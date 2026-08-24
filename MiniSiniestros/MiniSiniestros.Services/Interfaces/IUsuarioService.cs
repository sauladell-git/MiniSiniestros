using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<ServiceResponse<UsuarioDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
