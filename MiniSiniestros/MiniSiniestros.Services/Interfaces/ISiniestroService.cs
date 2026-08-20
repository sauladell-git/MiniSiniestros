using MiniSiniestros.Common.Paging;
using MiniSiniestros.Common.Responses;
using MiniSiniestros.Dto.Siniestro;

namespace MiniSiniestros.Services.Interfaces
{
    public interface ISiniestroService
    {
        Task<ServiceResponse<IReadOnlyList<SiniestroDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<SiniestroDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<SiniestroDto>> CreateAsync(CreateSiniestroDto dto, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> CambiarEstadoAsync(int siniestroId, int nuevoEstadoId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<PagedResponse<SiniestroDto>>> GetPagedAsync(SiniestroFilterRequest filter, CancellationToken cancellationToken = default);
        Task<ServiceResponse<bool>> AsignarPrestadorAsync(int siniestroId, int prestadorId, CancellationToken cancellationToken = default);
    }
}
