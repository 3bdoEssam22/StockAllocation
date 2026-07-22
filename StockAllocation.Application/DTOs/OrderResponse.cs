using System;
using System.Collections.Generic;
using System.Text;

namespace StockAllocation.Application.DTOs
{
    public record OrderResponse
    {
        public required string OrderNumber { get; init; }

        public required string Status { get; init; }

        public DateTime CreatedAtUtc { get; init; }

        public DateTime? CancelledAtUtc { get; init; }

        public required List<OrderLineResponse> Lines { get; init; }
    }
}
