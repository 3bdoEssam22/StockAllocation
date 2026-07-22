using Microsoft.AspNetCore.Mvc;
using StockAllocation.Application.Abstractions;
using StockAllocation.Application.Common;
using StockAllocation.Application.DTOs;

namespace StockAllocation.API.Controllers
{
    public class StockController(IStockAllocationService _stockService) : BaseApiController
    {
        [HttpGet("{sku}")]
        public async Task<ActionResult<GenericResponse<StockResponse>>> GetStock(string sku, CancellationToken ct)
        {
            var response = await _stockService.GetStockAsync(sku, ct);
            return HandleResponse(response);
        }
    }
}
