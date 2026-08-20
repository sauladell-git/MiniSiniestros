using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface IEmpleadorRepository : IRepository<Empleador>
    {
        Task<Empleador?> GetByCuitAsync(string cuit, CancellationToken cancellationToken = default);
    }
}
