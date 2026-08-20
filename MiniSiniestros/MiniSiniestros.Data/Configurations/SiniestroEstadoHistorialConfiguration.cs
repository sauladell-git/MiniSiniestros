using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class SiniestroEstadoHistorialConfiguration : IEntityTypeConfiguration<SiniestroEstadoHistorial>
    {
        public void Configure(EntityTypeBuilder<SiniestroEstadoHistorial> builder)
        {
            builder.ToTable("SiniestroEstadoHistoriales");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Fecha)
                .IsRequired();

            builder.HasOne(h => h.Siniestro)
                .WithMany()
                .HasForeignKey(h => h.SiniestroId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(h => h.SiniestroEstado)
                .WithMany()
                .HasForeignKey(h => h.SiniestroEstadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
