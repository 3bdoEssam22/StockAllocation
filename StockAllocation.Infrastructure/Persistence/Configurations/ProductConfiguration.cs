using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAllocation.Domain.Entities;
using StockAllocation.Infrastructure.Data.DataSeeding;

namespace StockAllocation.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => p.Sku).IsUnique();
            builder.Property(p => p.Sku).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();

            builder.HasData(SeedData.Products);
        }
    }
}