using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class SiniestroPrestadorRepository : Repository<Siniestro_Prestador>, ISiniestroPrestadorRepository
    {
        public SiniestroPrestadorRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Siniestro_Prestador>> GetPrestadoresPorSiniestroAsync(int siniestroId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(sp => sp.Prestador)
                .Where(sp => sp.SiniestroId == siniestroId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro_Prestador>> GetSiniestrosPorPrestadorAsync(int prestadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(sp => sp.Siniestro)
                .Where(sp => sp.PrestadorId == prestadorId)
                .ToListAsync(cancellationToken);
        }
    }
}
