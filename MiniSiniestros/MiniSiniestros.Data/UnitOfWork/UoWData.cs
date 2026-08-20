using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Implementations;
using MiniSiniestros.Data.Repositories.Interfaces;

namespace MiniSiniestros.Data.UnitOfWork
{
    public class UoWData : IUoWData
    {
        private readonly MiniSiniestrosDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private IDbContextTransaction? _currentTransaction;
        private bool _disposed;

        private IEmpleadorRepository? _empleadores;
        private IPrestadorRepository? _prestadores;
        private ISiniestroRepository? _siniestros;
        private ISiniestroEstadoRepository? _siniestroEstados;
        private ISiniestroEstadoHistorialRepository? _siniestroEstadoHistoriales;
        private ISiniestroPrestadorRepository? _siniestroPrestadores;
        private ITrabajadorRepository? _trabajadores;
        private IUsuarioRepository? _usuarios;

        public UoWData(MiniSiniestrosDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        public IEmpleadorRepository Empleadores =>
            _empleadores ??= new EmpleadorRepository(_context);

        public IPrestadorRepository Prestadores =>
            _prestadores ??= new PrestadorRepository(_context);

        public ISiniestroRepository Siniestros =>
            _siniestros ??= new SiniestroRepository(_context);

        public ISiniestroEstadoRepository SiniestroEstados =>
            _siniestroEstados ??= new SiniestroEstadoRepository(_context);

        public ISiniestroEstadoHistorialRepository SiniestroEstadoHistoriales =>
            _siniestroEstadoHistoriales ??= new SiniestroEstadoHistorialRepository(_context);

        public ISiniestroPrestadorRepository SiniestroPrestadores =>
            _siniestroPrestadores ??= new SiniestroPrestadorRepository(_context);

        public ITrabajadorRepository Trabajadores =>
            _trabajadores ??= new TrabajadorRepository(_context);

        public IUsuarioRepository Usuarios =>
            _usuarios ??= new UsuarioRepository(_context);

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return (IRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new Repository<TEntity>(_context));
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await CompleteAsync(cancellationToken);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public int SaveChanges()
        {
            return Complete();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return _currentTransaction;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _currentTransaction?.Dispose();
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }

                await _context.DisposeAsync();
                _disposed = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
