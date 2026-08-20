using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface ISiniestroEstadoHistorialRepository : IRepository<SiniestroEstadoHistorial>
    {
        Task<IReadOnlyList<SiniestroEstadoHistorial>> GetHistorialPorSiniestroAsync(int siniestroId, CancellationToken cancellationToken = default);
    }
}
