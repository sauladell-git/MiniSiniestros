using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class SiniestroRepository : Repository<Siniestro>, ISiniestroRepository
    {
        public SiniestroRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<Siniestro?> GetByIdConDetallesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetAllConDetallesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorEmpleadorAsync(int empleadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .Where(s => s.EmpleadorId == empleadorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorTrabajadorAsync(int trabajadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.SiniestroEstado)
                .Where(s => s.TrabajadorId == trabajadorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorEstadoAsync(int estadoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Where(s => s.SiniestroEstadoId == estadoId)
                .ToListAsync(cancellationToken);
        }
    }
}
