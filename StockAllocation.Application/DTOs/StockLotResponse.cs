using System;
using System.Collections.Generic;
using System.Text;

namespace StockAllocation.Application.DTOs
{
    public record StockLotResponse
    {
        public required string BatchCode { get; init; }

        public int QuantityOnHand { get; init; }

        public DateOnly ExpiresOn { get; init; }
    }
}
