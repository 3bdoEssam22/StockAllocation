using System;
using System.Collections.Generic;
using System.Text;

namespace StockAllocation.Application.DTOs
{
    public class CreateOrderRequest
    {
        public required string OrderNumber { get; init; }

        public required List<CreateOrderItemRequest> Items { get; init; }
    }
}
