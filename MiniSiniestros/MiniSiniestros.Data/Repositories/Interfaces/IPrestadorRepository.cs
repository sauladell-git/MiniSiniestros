using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface IPrestadorRepository : IRepository<Prestador>
    {
        Task<IReadOnlyList<Prestador>> GetByNombreAsync(string nombre, CancellationToken cancellationToken = default);
    }
}
