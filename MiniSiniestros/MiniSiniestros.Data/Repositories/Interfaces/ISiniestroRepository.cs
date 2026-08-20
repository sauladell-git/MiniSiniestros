using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface ISiniestroRepository : IRepository<Siniestro>
    {
        Task<Siniestro?> GetByIdConDetallesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Siniestro>> GetAllConDetallesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Siniestro>> GetPorEmpleadorAsync(int empleadorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Siniestro>> GetPorTrabajadorAsync(int trabajadorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Siniestro>> GetPorEstadoAsync(int estadoId, CancellationToken cancellationToken = default);
    }
}
