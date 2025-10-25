using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAPI.Domain.Entities;

namespace PruebaAPI.Infrastructure.Persistence.Configurations;

public class MarcaAutoConfiguration : IEntityTypeConfiguration<MarcaAuto>
{
    public void Configure(EntityTypeBuilder<MarcaAuto> builder)
    {
        builder.ToTable("MarcasAutos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.PaisOrigen)
            .HasMaxLength(100);

        builder.Property(m => m.AñoFundacion)
            .IsRequired();

        builder.Property(m => m.SitioWeb)
            .HasMaxLength(200);

        builder.Property(m => m.EsActiva)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(m => m.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(m => m.Nombre);
        builder.HasIndex(m => m.PaisOrigen);
    }
}
