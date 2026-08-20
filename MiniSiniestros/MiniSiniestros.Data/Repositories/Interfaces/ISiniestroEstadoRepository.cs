using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface ISiniestroEstadoRepository : IRepository<SiniestroEstado>
    {
        Task<SiniestroEstado?> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default);
    }
}
