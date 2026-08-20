using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class TrabajadorRepository : Repository<Trabajador>, ITrabajadorRepository
    {
        public TrabajadorRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<Trabajador?> GetByCuilAsync(string cuil, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Empleador)
                .FirstOrDefaultAsync(t => t.Cuil == cuil, cancellationToken);
        }

        public async Task<IReadOnlyList<Trabajador>> GetPorEmpleadorAsync(int empleadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(t => t.EmpleadorId == empleadorId)
                .ToListAsync(cancellationToken);
        }
    }
}
