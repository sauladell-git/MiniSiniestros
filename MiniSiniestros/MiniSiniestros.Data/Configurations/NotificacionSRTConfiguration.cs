using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class NotificacionSRTConfiguration : IEntityTypeConfiguration<NotificacionSRT>
    {
        public void Configure(EntityTypeBuilder<NotificacionSRT> builder)
        {
            builder.ToTable("NotificacionesSRT");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Timestamp)
                .IsRequired();

            builder.Property(n => n.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Payload)
                .IsRequired();

            builder.Property(n => n.Intentos)
                .IsRequired();

            builder.HasOne(n => n.Siniestro)
                .WithMany()
                .HasForeignKey(n => n.SiniestroId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
