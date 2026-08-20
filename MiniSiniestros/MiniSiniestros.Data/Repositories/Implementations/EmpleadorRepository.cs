using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class EmpleadorRepository : Repository<Empleador>, IEmpleadorRepository
    {
        public EmpleadorRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<Empleador?> GetByCuitAsync(string cuit, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Cuit == cuit, cancellationToken);
        }
    }
}
