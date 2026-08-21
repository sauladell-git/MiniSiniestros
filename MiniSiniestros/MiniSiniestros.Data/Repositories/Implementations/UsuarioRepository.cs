using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<Usuario?> GetByNombreYApellidoAsync(string nombre, string apellido, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Nombre == nombre && u.Apellido == apellido, cancellationToken);
        }

        public async Task<Usuario?> GetByNombreConRolesAsync(string nombre, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Nombre == nombre, cancellationToken);
        }
    }
}
