using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniSiniestros.Data.Context;
using MiniSiniestros.Data.Repositories.Implementations;
using MiniSiniestros.Data.Repositories.Interfaces;
using MiniSiniestros.Data.UnitOfWork;

namespace MiniSiniestros.Data.Extensions
{
    public static class DataServiceRegistration
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada!");

            services.AddDbContext<MiniSiniestrosDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("MiniSiniestros.Data.Migrations")));

            // registramos el generico
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // registramos los especificos 
            services.AddScoped<IEmpleadorRepository, EmpleadorRepository>();
            services.AddScoped<IPrestadorRepository, PrestadorRepository>();
            services.AddScoped<ISiniestroRepository, SiniestroRepository>();
            services.AddScoped<ISiniestroEstadoRepository, SiniestroEstadoRepository>();
            services.AddScoped<ISiniestroEstadoHistorialRepository, SiniestroEstadoHistorialRepository>();
            services.AddScoped<ISiniestroPrestadorRepository, SiniestroPrestadorRepository>();
            services.AddScoped<ITrabajadorRepository, TrabajadorRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // registrar el UnitofWork!
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}
