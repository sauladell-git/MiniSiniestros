using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByNombreYApellidoAsync(string nombre, string apellido, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByNombreConRolesAsync(string nombre, CancellationToken cancellationToken = default);
        Task<Usuario?> GetByIdConRolesAsync(int id, CancellationToken cancellationToken = default);
    }
}
