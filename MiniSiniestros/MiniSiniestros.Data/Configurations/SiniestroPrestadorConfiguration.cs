using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniSiniestros.Entities;

namespace MiniSiniestros.Data.Configurations
{
    public class SiniestroPrestadorConfiguration : IEntityTypeConfiguration<Siniestro_Prestador>
    {
        public void Configure(EntityTypeBuilder<Siniestro_Prestador> builder)
        {
            builder.ToTable("SiniestroPrestadores");

            builder.HasKey(sp => sp.Id);

            builder.HasOne(sp => sp.Siniestro)
                .WithMany()
                .HasForeignKey(sp => sp.SiniestroId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sp => sp.Prestador)
                .WithMany()
                .HasForeignKey(sp => sp.PrestadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
