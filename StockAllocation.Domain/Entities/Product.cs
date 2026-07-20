namespace StockAllocation.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Sku { get; set; } = null!;

        public string Name { get; set; } = null!;

        public ICollection<StockLot> StockLots { get; set; } = [];

        public ICollection<OrderLine> OrderLines { get; set; } = [];
    }
}
