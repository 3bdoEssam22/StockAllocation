namespace StockAllocation.Domain.Entities
{
    public class StockLot
    {
        public int Id { get; set; }

        public string BatchCode { get; set; } = null!;

        public int QuantityOnHand { get; set; }

        public DateOnly ExpiresOn { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public ICollection<Allocation> Allocations { get; set; } = [];
    }
}