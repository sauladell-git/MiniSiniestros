using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class UsuarioRolConfiguration : IEntityTypeConfiguration<Usuario_Rol>
    {
        public void Configure(EntityTypeBuilder<Usuario_Rol> builder)
        {
            builder.ToTable("Usuario_Rol");

            builder.HasKey(ur => new { ur.UsuarioId, ur.RolId });

            builder.HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(ur => ur.UsuarioId);

            builder.HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.RolId);
        }
    }
}
