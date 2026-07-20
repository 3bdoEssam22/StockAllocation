namespace StockAllocation.Domain.Entities
{
    public class OrderLine
    {
        public int Id { get; set; }

        public int RequestedQuantity { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public ICollection<Allocation> Allocations { get; set; } = [];
    }
}