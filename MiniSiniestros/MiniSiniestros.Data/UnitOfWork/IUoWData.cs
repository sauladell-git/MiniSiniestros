using Microsoft.EntityFrameworkCore.Storage;
using MiniSiniestros.Data.Repositories.Interfaces;

namespace MiniSiniestros.Data.UnitOfWork
{
    public interface IUoWData : IDisposable, IAsyncDisposable
    {
        IEmpleadorRepository Empleadores { get; }
        INotificacionSRTRepository NotificacionesSRT { get; }
        IPrestadorRepository Prestadores { get; }
        ISiniestroRepository Siniestros { get; }
        ISiniestroEstadoRepository SiniestroEstados { get; }
        ISiniestroEstadoHistorialRepository SiniestroEstadoHistoriales { get; }
        ISiniestroPrestadorRepository SiniestroPrestadores { get; }
        ITrabajadorRepository Trabajadores { get; }
        IUsuarioRepository Usuarios { get; }

        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int Complete();
        int SaveChanges();

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
