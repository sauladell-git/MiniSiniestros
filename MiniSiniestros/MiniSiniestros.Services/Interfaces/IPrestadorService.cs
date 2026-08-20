using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Prestador;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IPrestadorService
    {
        Task<ServiceResponse<IReadOnlyList<PrestadorDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<PrestadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
