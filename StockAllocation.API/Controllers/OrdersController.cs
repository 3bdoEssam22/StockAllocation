using Microsoft.AspNetCore.Mvc;
using StockAllocation.Application.Abstractions;
using StockAllocation.Application.Common;
using StockAllocation.Application.DTOs;

namespace StockAllocation.API.Controllers
{
    public class OrderController(IStockAllocationService _stockService) : BaseApiController
    {
        // POST /api/order
        [HttpPost]
        public async Task<ActionResult<GenericResponse<OrderResponse>>> CreateOrder(CreateOrderRequest request, CancellationToken ct)
        {
            var response = await _stockService.AllocateOrderAsync(request, ct);
            return HandleResponse(response);
        }

        // GET /api/order/{orderNumber}
        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<GenericResponse<OrderResponse>>> GetOrder(string orderNumber, CancellationToken ct)
        {
            var response = await _stockService.GetOrderAsync(orderNumber, ct);
            return HandleResponse(response);
        }

        //POST /api/order/{orderNumber}/cancel 
        [HttpPost("{orderNumber}/cancel")]
        public async Task<ActionResult<GenericResponse<OrderResponse>>> CancelOrder(string orderNumber, CancellationToken ct)
        {
            var response = await _stockService.CancelOrderAsync(orderNumber, ct);
            return HandleResponse(response);
        }

    }
}