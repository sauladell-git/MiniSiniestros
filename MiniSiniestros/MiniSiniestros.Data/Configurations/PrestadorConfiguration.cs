using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class PrestadorConfiguration : IEntityTypeConfiguration<Prestador>
    {
        public void Configure(EntityTypeBuilder<Prestador> builder)
        {
            builder.ToTable("Prestadores");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
