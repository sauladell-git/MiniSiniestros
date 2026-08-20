using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class TrabajadorConfiguration : IEntityTypeConfiguration<Trabajador>
    {
        public void Configure(EntityTypeBuilder<Trabajador> builder)
        {
            builder.ToTable("Trabajadores");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.Apellido)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.Cuil)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(t => t.Empleador)
                .WithMany()
                .HasForeignKey(t => t.EmpleadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
