using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class SiniestroEstadoHistorialRepository : Repository<SiniestroEstadoHistorial>, ISiniestroEstadoHistorialRepository
    {
        public SiniestroEstadoHistorialRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<SiniestroEstadoHistorial>> GetHistorialPorSiniestroAsync(int siniestroId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(h => h.SiniestroEstado)
                .Where(h => h.SiniestroId == siniestroId)
                .OrderByDescending(h => h.Fecha)
                .ToListAsync(cancellationToken);
        }
    }
}
