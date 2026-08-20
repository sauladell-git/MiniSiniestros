using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class SiniestroEstadoConfiguration : IEntityTypeConfiguration<SiniestroEstado>
    {
        public void Configure(EntityTypeBuilder<SiniestroEstado> builder)
        {
            builder.ToTable("SiniestroEstados");

            builder.HasKey(se => se.Id);

            builder.Property(se => se.Nombre)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
