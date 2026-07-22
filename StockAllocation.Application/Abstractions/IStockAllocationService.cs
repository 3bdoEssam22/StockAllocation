using StockAllocation.Application.Common;
using StockAllocation.Application.DTOs;

namespace StockAllocation.Application.Abstractions
{
    public interface IStockAllocationService
    {
        Task<GenericResponse<OrderResponse>> AllocateOrderAsync(CreateOrderRequest request, CancellationToken ct);
        Task<GenericResponse<OrderResponse>> CancelOrderAsync(string orderNumber, CancellationToken ct);
        Task<GenericResponse<OrderResponse>> GetOrderAsync(string orderNumber, CancellationToken ct);
        Task<GenericResponse<StockResponse>> GetStockAsync(string sku, CancellationToken ct);
    }
}