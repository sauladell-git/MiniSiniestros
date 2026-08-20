using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Repositories.Implementations
{
    public class SiniestroRepository : Repository<Siniestro>, ISiniestroRepository
    {
        public SiniestroRepository(MiniSiniestrosDbContext context) : base(context)
        {
        }

        public async Task<Siniestro?> GetByIdConDetallesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetAllConDetallesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorEmpleadorAsync(int empleadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .Where(s => s.EmpleadorId == empleadorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorTrabajadorAsync(int trabajadorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.SiniestroEstado)
                .Where(s => s.TrabajadorId == trabajadorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Siniestro>> GetPorEstadoAsync(int estadoId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Where(s => s.SiniestroEstadoId == estadoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUltimoNumeroAsync(CancellationToken cancellationToken = default)
        {
            var maxNumero = await _dbSet
                .AsNoTracking()
                .Select(s => (int?)s.Numero)
                .MaxAsync(cancellationToken);

            return maxNumero ?? 1000;
        }

        public async Task<(IReadOnlyList<Siniestro> Items, int TotalCount)> GetPagedAsync(SiniestroFilterRequest filter, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(s => s.Empleador)
                .Include(s => s.Trabajador)
                .Include(s => s.SiniestroEstado)
                .AsQueryable();

            // 1. Filtrar por CUIT de Empleador
            if (!string.IsNullOrWhiteSpace(filter.Cuit))
            {
                var cuitClean = filter.Cuit.Trim();
                var cuitNumeric = cuitClean.Replace("-", "");
                query = query.Where(s => s.Empleador.Cuit == cuitClean || s.Empleador.Cuit.Replace("-", "") == cuitNumeric);
            }

            // 2. Filtrar por CUIL de Trabajador
            if (!string.IsNullOrWhiteSpace(filter.Cuil))
            {
                var cuilClean = filter.Cuil.Trim();
                var cuilNumeric = cuilClean.Replace("-", "");
                query = query.Where(s => s.Trabajador.Cuil == cuilClean || s.Trabajador.Cuil.Replace("-", "") == cuilNumeric);
            }

            // 3. Filtrar por Rango de Fechas
            if (filter.FechaDesde.HasValue)
            {
                query = query.Where(s => s.Fecha >= filter.FechaDesde.Value);
            }

            if (filter.FechaHasta.HasValue)
            {
                query = query.Where(s => s.Fecha <= filter.FechaHasta.Value);
            }

            // 4. Filtrar por EstadoId
            if (filter.SiniestroEstadoId.HasValue && filter.SiniestroEstadoId.Value > 0)
            {
                query = query.Where(s => s.SiniestroEstadoId == filter.SiniestroEstadoId.Value);
            }

            // Conteo total antes de paginar
            var totalCount = await query.CountAsync(cancellationToken);

            // 5. Ordenamiento
            var sortBy = filter.SortBy?.Trim().ToLowerInvariant() ?? "fecha";
            if (sortBy == "estado" || sortBy == "siniestroestado")
            {
                query = filter.IsDescending
                    ? query.OrderByDescending(s => s.SiniestroEstado.Nombre).ThenByDescending(s => s.Fecha)
                    : query.OrderBy(s => s.SiniestroEstado.Nombre).ThenBy(s => s.Fecha);
            }
            else // Por defecto "fecha"
            {
                query = filter.IsDescending
                    ? query.OrderByDescending(s => s.Fecha)
                    : query.OrderBy(s => s.Fecha);
            }

            // 6. Paginación
            var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
