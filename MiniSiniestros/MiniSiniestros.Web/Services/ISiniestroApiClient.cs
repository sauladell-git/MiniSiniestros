using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Auth;
using MiniSiniestros.ViewModels.Siniestros;

namespace MiniSiniestros.Web.Services
{
    public interface ISiniestroApiClient
    {
        Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResponse<SiniestroListViewModel>> GetPagedSiniestrosAsync(SiniestroFilterViewModel filter, CancellationToken cancellationToken = default);
        Task<ServiceResponse<SiniestroDetailViewModel>> GetSiniestroByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
