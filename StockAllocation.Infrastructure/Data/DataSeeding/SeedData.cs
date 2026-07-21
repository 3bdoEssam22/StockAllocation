using StockAllocation.Domain.Entities;

namespace StockAllocation.Infrastructure.Data.DataSeeding
{
    public static class SeedData
    {
        public static readonly Product[] Products =
        [
            new Product { Id = 1, Sku = "PEN-001", Name = "Blue Pen" },
            new Product { Id = 2, Sku = "NOTE-001", Name = "Notebook" },
            new Product { Id = 3, Sku = "MUG-001", Name = "Company Mug" }
        ];

        public static readonly StockLot[] StockLots =
        [
            new StockLot { Id = 1, BatchCode = "PEN-OLD", QuantityOnHand = 100, ExpiresOn = new DateOnly(2000, 1, 1), ProductId = 1 },
            new StockLot { Id = 2, BatchCode = "PEN-A", QuantityOnHand = 3, ExpiresOn = new DateOnly(2099, 1, 10), ProductId = 1 },
            new StockLot { Id = 3, BatchCode = "PEN-B", QuantityOnHand = 5, ExpiresOn = new DateOnly(2099, 2, 10), ProductId = 1 },
            new StockLot { Id = 4, BatchCode = "NOTE-A", QuantityOnHand = 2, ExpiresOn = new DateOnly(2099, 1, 5), ProductId = 2 },
            new StockLot { Id = 5, BatchCode = "NOTE-B", QuantityOnHand = 10, ExpiresOn = new DateOnly(2099, 3, 1), ProductId = 2 },
            new StockLot { Id = 6, BatchCode = "MUG-A", QuantityOnHand = 0, ExpiresOn = new DateOnly(2099, 12, 31), ProductId = 3 }
        ];
    }
}