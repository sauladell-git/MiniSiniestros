using System.Linq.Expressions;

namespace MiniSiniestros.Data.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);

        Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);

        IQueryable<T> Query(
            Expression<Func<T, bool>>? predicate = null,
            bool disableTracking = true);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        void Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }
}
