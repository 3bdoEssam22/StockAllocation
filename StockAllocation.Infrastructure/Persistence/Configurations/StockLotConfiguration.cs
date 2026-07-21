using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAllocation.Domain.Entities;
using StockAllocation.Infrastructure.Data.DataSeeding;

namespace StockAllocation.Infrastructure.Persistence.Configurations
{
    public class StockLotConfiguration : IEntityTypeConfiguration<StockLot>
    {
        public void Configure(EntityTypeBuilder<StockLot> builder)
        {
            builder.HasKey(sl => sl.Id);

            builder.Property(sl => sl.BatchCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sl => sl.QuantityOnHand)
                .IsRequired();

            builder.Property(sl => sl.ExpiresOn)
                .IsRequired()
                .HasColumnType("date");


            builder.HasIndex(sl => new { sl.ProductId, sl.BatchCode })
                .IsUnique();

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_StockLot_QuantityOnHand_NonNegative",
                "[QuantityOnHand] >= 0"));

            builder.HasOne(sl => sl.Product)
                .WithMany(p => p.StockLots)
                .HasForeignKey(sl => sl.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(SeedData.StockLots);
        }
    }
}