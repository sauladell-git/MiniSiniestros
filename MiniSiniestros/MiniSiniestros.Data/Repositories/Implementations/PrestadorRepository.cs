using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class PrestadorRepository : Repository<Prestador>, IPrestadorRepository
    {
        public PrestadorRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Prestador>> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Nombre.Contains(nombre))
                .ToListAsync(cancellationToken);
        }
    }
}
