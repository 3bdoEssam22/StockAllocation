using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAllocation.Domain.Entities;

namespace StockAllocation.Infrastructure.Persistence.Configurations
{
    public class AllocationConfiguration : IEntityTypeConfiguration<Allocation>
    {
        public void Configure(EntityTypeBuilder<Allocation> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Quantity)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Allocation_Quantity_Positive",
                "[Quantity] > 0"));

            builder.HasOne(a => a.OrderLine)
                .WithMany(ol => ol.Allocations)
                .HasForeignKey(a => a.OrderLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.StockLot)
                .WithMany(sl => sl.Allocations)
                .HasForeignKey(a => a.StockLotId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
