using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;

namespace MiniSiniestros.Services.Interfaces
{
    public interface ISiniestroEstadoService
    {
        Task<ServiceResponse<SiniestroEstadoDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IReadOnlyList<SiniestroEstadoDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> ExisteEstadoAsync(int id, CancellationToken cancellationToken = default);
    }
}
