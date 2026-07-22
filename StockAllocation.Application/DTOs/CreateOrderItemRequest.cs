namespace StockAllocation.Application.DTOs
{
    public record CreateOrderItemRequest
    {
        public required string Sku { get; init; }

        public int Quantity { get; init; }
    }
}