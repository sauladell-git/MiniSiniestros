using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    }
}
