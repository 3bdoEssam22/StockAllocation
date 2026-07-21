using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAllocation.Domain.Entities;

namespace StockAllocation.Infrastructure.Persistence.Configurations
{
    public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
    {

        public void Configure(EntityTypeBuilder<OrderLine> builder)
        {
            builder.HasKey(ol => ol.Id);

            builder.Property(ol => ol.RequestedQuantity)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
                    "CK_OrderLine_RequestedQuantity_Positive",
                    "[RequestedQuantity] > 0"));

            builder.HasIndex(ol => new { ol.OrderId, ol.ProductId })
                .IsUnique();

            builder.HasOne(ol => ol.Order)
                    .WithMany(o => o.OrderLines)
                    .HasForeignKey(ol => ol.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ol => ol.Product)
                    .WithMany(p => p.OrderLines)
                    .HasForeignKey(ol => ol.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
