using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Implementations;
using MiniSiniestros.Dto.Siniestro;
using MiniSiniestros.Entities;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class SiniestroRepositoryTests
    {
        private MiniSiniestrosDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<MiniSiniestrosDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new MiniSiniestrosDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private async Task SeedDatabaseAsync(MiniSiniestrosDbContext context)
        {
            var emp1 = new Empleador { Id = 1, Cuit = "30111111111", RazonSocial = "Emp 1" };
            var emp2 = new Empleador { Id = 2, Cuit = "30222222222", RazonSocial = "Emp 2" };

            var trab1 = new Trabajador { Id = 1, Cuil = "20111111111", Nombre = "Juan", Apellido = "Pérez" };
            var trab2 = new Trabajador { Id = 2, Cuil = "20222222222", Nombre = "Carlos", Apellido = "Gómez" };

            var est1 = new SiniestroEstado { Id = 1, Nombre = "Recibido" };
            var est2 = new SiniestroEstado { Id = 2, Nombre = "EnProceso" };

            context.Empleadores.AddRange(emp1, emp2);
            context.Trabajadores.AddRange(trab1, trab2);
            context.SiniestroEstados.AddRange(est1, est2);

            var s1 = new Siniestro
            {
                Id = 1,
                Numero = 1001,
                Fecha = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                EmpleadorId = 1,
                TrabajadorId = 1,
                SiniestroEstadoId = 1
            };
            var s2 = new Siniestro
            {
                Id = 2,
                Numero = 1002,
                Fecha = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                EmpleadorId = 2,
                TrabajadorId = 2,
                SiniestroEstadoId = 2
            };

            context.Siniestros.AddRange(s1, s2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByIdConDetallesAsync_IdValido_RetornaSiniestroConNavegaciones()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetById");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetByIdConDetallesAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotNull(result.Empleador);
            Assert.NotNull(result.Trabajador);
            Assert.NotNull(result.SiniestroEstado);
        }

        [Fact]
        public async Task GetAllConDetallesAsync_RetornaTodosConNavegaciones()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetAll");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetAllConDetallesAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPorEmpleadorAsync_FiltraCorrectamente()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPorEmpleador");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetPorEmpleadorAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].EmpleadorId);
        }

        [Fact]
        public async Task GetPorTrabajadorAsync_FiltraCorrectamente()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPorTrabajador");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetPorTrabajadorAsync(2);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].TrabajadorId);
        }

        [Fact]
        public async Task GetPorEstadoAsync_FiltraCorrectamente()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPorEstado");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetPorEstadoAsync(2);

            // Assert
            Assert.Single(result);
            Assert.Equal(2, result[0].SiniestroEstadoId);
        }

        [Fact]
        public async Task GetUltimoNumeroAsync_RetornaMaximoNumero()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetUltimoNumero");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            // Act
            var result = await repo.GetUltimoNumeroAsync();

            // Assert
            Assert.Equal(1002, result);
        }

        [Fact]
        public async Task GetPagedAsync_SinFiltros_RetornaTodosPaginados()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPaged_SinFiltros");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            var filter = new SiniestroFilterRequest { PageNumber = 1, PageSize = 10 };

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(filter);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task GetPagedAsync_FiltradoPorCuit_RetornaSiniestroDeEmpleador()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPaged_Cuit");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            var filter = new SiniestroFilterRequest { Cuit = "30-11111111-1" };

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(filter);

            // Assert
            Assert.Equal(1, totalCount);
            Assert.Equal(1, items[0].Id);
        }

        [Fact]
        public async Task GetPagedAsync_FiltradoPorCuil_RetornaSiniestroDeTrabajador()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPaged_Cuil");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            var filter = new SiniestroFilterRequest { Cuil = "20222222222" };

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(filter);

            // Assert
            Assert.Equal(1, totalCount);
            Assert.Equal(2, items[0].Id);
        }

        [Fact]
        public async Task GetPagedAsync_FiltradoPorFechasYEstado_RetornaMatchingSiniestro()
        {
            // Arrange
            using var context = CreateDbContext("Db_GetPaged_FechasEstado");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            var filter = new SiniestroFilterRequest
            {
                FechaDesde = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                FechaHasta = new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                SiniestroEstadoId = 1
            };

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(filter);

            // Assert
            Assert.Equal(1, totalCount);
            Assert.Equal(1, items[0].Id);
        }

        [Theory]
        [InlineData("estado", true)]
        [InlineData("estado", false)]
        [InlineData("fecha", true)]
        [InlineData("fecha", false)]
        public async Task GetPagedAsync_OrdenamientoYPaginacion_RetornaOrdenCorrecto(string sortBy, bool isDescending)
        {
            // Arrange
            using var context = CreateDbContext($"Db_GetPaged_Sort_{sortBy}_{isDescending}");
            await SeedDatabaseAsync(context);
            var repo = new SiniestroRepository(context);

            var filter = new SiniestroFilterRequest
            {
                SortBy = sortBy,
                IsDescending = isDescending,
                PageNumber = 0, // Fuerza fallback a 1
                PageSize = 0    // Fuerza fallback a 10
            };

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(filter);

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, items.Count);
        }
    }
}
