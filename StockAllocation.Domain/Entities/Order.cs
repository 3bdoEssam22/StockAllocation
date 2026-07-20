using StockAllocation.Domain.Entities.enums;

namespace StockAllocation.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = null!;

        public OrderStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; } = [];
    }
}