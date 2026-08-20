using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class SiniestroConfiguration : IEntityTypeConfiguration<Siniestro>
    {
        public void Configure(EntityTypeBuilder<Siniestro> builder)
        {
            builder.ToTable("Siniestros");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Numero)
                .IsRequired();

            builder.Property(s => s.Fecha)
                .IsRequired();

            builder.Property(s => s.Observaciones)
                .HasMaxLength(500);

            builder.HasOne(s => s.Empleador)
                .WithMany()
                .HasForeignKey(s => s.EmpleadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Trabajador)
                .WithMany()
                .HasForeignKey(s => s.TrabajadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.SiniestroEstado)
                .WithMany()
                .HasForeignKey(s => s.SiniestroEstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
