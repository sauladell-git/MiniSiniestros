using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class EmpleadorConfiguration : IEntityTypeConfiguration<Empleador>
    {
        public void Configure(EntityTypeBuilder<Empleador> builder)
        {
            builder.ToTable("Empleadores");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RazonSocial)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Cuit)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(e => e.Cuit)
                .IsUnique();
        }
    }
}
