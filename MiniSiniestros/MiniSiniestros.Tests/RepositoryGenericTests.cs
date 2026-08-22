using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Implementations;
using MiniSiniestros.Entities;
using Xunit;

namespace MiniSiniestros.Tests
{
    public class RepositoryGenericTests
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

        [Fact]
        public async Task Repository_CrudOperations_WorkAsExpected()
        {
            // Arrange
            using var context = CreateDbContext("Db_RepositoryGeneric");
            var repo = new Repository<Empleador>(context);

            var emp = new Empleador { Id = 1, Cuit = "30111111111", RazonSocial = "Emp Gen 1" };

            // 1. AddAsync
            await repo.AddAsync(emp);
            await context.SaveChangesAsync();

            // 2. ExistsAsync
            var exists = await repo.ExistsAsync(e => e.Cuit == "30111111111");
            Assert.True(exists);

            // 3. GetFirstOrDefaultAsync
            var found = await repo.GetFirstOrDefaultAsync(e => e.Id == 1);
            Assert.NotNull(found);
            Assert.Equal("Emp Gen 1", found.RazonSocial);

            // 4. GetAsync with predicate and ordering
            var list = await repo.GetAsync(
                predicate: e => e.Cuit.StartsWith("30"),
                orderBy: q => q.OrderBy(e => e.RazonSocial),
                includeProperties: null,
                disableTracking: true);
            Assert.Single(list);

            // 5. CountAsync
            var count = await repo.CountAsync(e => e.Id == 1);
            Assert.Equal(1, count);

            // 6. Update
            emp.RazonSocial = "Emp Gen Editado";
            repo.Update(emp);
            await context.SaveChangesAsync();
            var updated = await repo.GetByIdAsync(1);
            Assert.Equal("Emp Gen Editado", updated!.RazonSocial);

            // 7. Remove
            repo.Remove(emp);
            await context.SaveChangesAsync();
            var afterDelete = await repo.GetByIdAsync(1);
            Assert.Null(afterDelete);
        }
    }
}
