using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface ISiniestroPrestadorRepository : IRepository<Siniestro_Prestador>
    {
        Task<IReadOnlyList<Siniestro_Prestador>> GetPrestadoresPorSiniestroAsync(int siniestroId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Siniestro_Prestador>> GetSiniestrosPorPrestadorAsync(int prestadorId, CancellationToken cancellationToken = default);
    }
}
