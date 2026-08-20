using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface ITrabajadorRepository : IRepository<Trabajador>
    {
        Task<Trabajador?> GetByCuilAsync(string cuil, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Trabajador>> GetPorEmpleadorAsync(int empleadorId, CancellationToken cancellationToken = default);
    }
}
