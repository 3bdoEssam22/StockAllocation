namespace StockAllocation.Application.DTOs
{
    public record AllocationResponse
    {
        public required string BatchCode { get; init; }

        public int Quantity { get; init; }
    }
}