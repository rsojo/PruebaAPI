using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PruebaAPI.Domain.Entities;

namespace PruebaAPI.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasPrecision(18, 2);

        builder.Property(e => e.Stock)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Indexes
        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(e => e.Price)
            .HasDatabaseName("IX_Products_Price");

        // Seed Data
        builder.HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Description = "High performance laptop",
                Price = 1299.99m,
                Stock = 10,
                CreatedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 2,
                Name = "Mouse",
                Description = "Wireless mouse",
                Price = 29.99m,
                Stock = 50,
                CreatedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Description = "Mechanical keyboard",
                Price = 89.99m,
                Stock = 30,
                CreatedAt = new DateTime(2025, 10, 25, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
