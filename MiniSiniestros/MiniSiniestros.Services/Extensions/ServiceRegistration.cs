using Microsoft.Extensions.DependencyInjection;
using MiniSiniestros.Services.Implementations;
using MiniSiniestros.Services.Interfaces;

namespace MiniSiniestros.Services.Extensions
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ServiceRegistration).Assembly);

            services.AddScoped<IEmpleadorService, EmpleadorService>();
            services.AddScoped<IPrestadorService, PrestadorService>();
            services.AddScoped<ITrabajadorService, TrabajadorService>();
            services.AddScoped<ISiniestroEstadoService, SiniestroEstadoService>();
            services.AddScoped<ISiniestroService, SiniestroService>();
            services.AddScoped<IStrNotificationService, StrNotificationService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
