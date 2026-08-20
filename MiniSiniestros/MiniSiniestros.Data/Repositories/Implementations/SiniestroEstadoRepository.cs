using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class SiniestroEstadoRepository : Repository<SiniestroEstado>, ISiniestroEstadoRepository
    {
        public SiniestroEstadoRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<SiniestroEstado?> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(se => se.Nombre == nombre, cancellationToken);
        }
    }
}
