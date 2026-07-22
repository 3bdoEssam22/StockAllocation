namespace StockAllocation.Application.DTOs
{
    public record OrderLineResponse
    {
        public required string Sku { get; init; }

        public int RequestedQuantity { get; init; }

        public required List<AllocationResponse> Allocations { get; init; }
    }
}