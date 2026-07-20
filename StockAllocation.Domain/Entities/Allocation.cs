namespace StockAllocation.Domain.Entities
{
    public class Allocation
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int OrderLineId { get; set; }

        public OrderLine OrderLine { get; set; } = null!;

        public int StockLotId { get; set; }

        public StockLot StockLot { get; set; } = null!;
    }
}