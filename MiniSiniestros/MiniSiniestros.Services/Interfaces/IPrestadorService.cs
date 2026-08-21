using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Prestador;

namespace MiniSiniestros.Services.Interfaces
{
    public interface IPrestadorService
    {
        Task<ServiceResponse<PrestadorDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IReadOnlyList<PrestadorDto>>> GetPrestadoresPorSiniestrosAsync(int siniestroId, CancellationToken cancellationToken = default);
    }
}
