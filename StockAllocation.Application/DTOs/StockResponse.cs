namespace StockAllocation.Application.DTOs
{
    public record StockResponse
    {
        public required string Sku { get; init; }

        public required string Name { get; init; }

        public required List<StockLotResponse> Lots { get; init; }

        public int TotalEligibleQuantity { get; init; }
    }
}
