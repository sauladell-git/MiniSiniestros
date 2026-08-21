using MiniSiniestros.Common.Responses;
using MiniSiniestros.ViewModels.Siniestros;

namespace MiniSiniestros.Web.Services
{
    public interface ISiniestroApiClient
    {
        Task<ServiceResponse<SiniestroListViewModel>> GetPagedSiniestrosAsync(SiniestroFilterViewModel filter, CancellationToken cancellationToken = default);
        Task<ServiceResponse<SiniestroDetailViewModel>> GetSiniestroByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
