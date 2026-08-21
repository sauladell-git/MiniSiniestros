using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Context
{
    public class MiniSiniestrosDbContext : DbContext
    {
        public MiniSiniestrosDbContext(DbContextOptions<MiniSiniestrosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Empleador> Empleadores { get; set; } = null!;
        public DbSet<NotificacionSRT> NotificacionesSRT { get; set; } = null!;
        public DbSet<Prestador> Prestadores { get; set; } = null!;
        public DbSet<Rol> Roles { get; set; } = null!;
        public DbSet<Siniestro> Siniestros { get; set; } = null!;
        public DbSet<SiniestroEstado> SiniestroEstados { get; set; } = null!;
        public DbSet<SiniestroEstadoHistorial> SiniestroEstadoHistoriales { get; set; } = null!;
        public DbSet<Siniestro_Prestador> SiniestroPrestadores { get; set; } = null!;
        public DbSet<Trabajador> Trabajadores { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Usuario_Rol> UsuarioRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
