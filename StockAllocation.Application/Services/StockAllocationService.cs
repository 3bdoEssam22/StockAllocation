using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StockAllocation.Application.Abstractions;
using StockAllocation.Application.Common;
using StockAllocation.Application.DTOs;
using StockAllocation.Domain.Entities;
using StockAllocation.Domain.Entities.enums;

namespace StockAllocation.Application.Services
{
    public class StockAllocationService(IApplicationDbContext _context) : IStockAllocationService
    {

        public async Task<GenericResponse<OrderResponse>> AllocateOrderAsync(CreateOrderRequest request, CancellationToken ct)
        {
            var response = new GenericResponse<OrderResponse>();

            if (string.IsNullOrWhiteSpace(request.OrderNumber) || request.Items is null || request.Items.Count == 0)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Code = ErrorCodes.InvalidRequest;
                response.Message = "Order number is required and items cannot be empty.";
                return response;
            }

            if (request.Items.Any(i => i.Quantity <= 0) ||
                request.Items.Select(i => i.Sku).Distinct().Count() != request.Items.Count)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Code = ErrorCodes.InvalidRequest;
                response.Message = "Quantities must be positive and SKUs must be unique within the order.";
                return response;
            }

            if (await _context.Orders.AnyAsync(o => o.OrderNumber == request.OrderNumber, ct))
            {
                response.StatusCode = StatusCodes.Status409Conflict;
                response.Code = ErrorCodes.DuplicateOrderNumber;
                response.Message = $"Order number '{request.OrderNumber}' already exists.";
                return response;
            }

            var requestedSkus = request.Items
                .Select(item => item.Sku)
                .ToList();

            var products = await _context.Products
                .Where(product => requestedSkus.Contains(product.Sku))
                .ToListAsync(ct);

            var productsBySku = products
                .ToDictionary(product => product.Sku);

            var unknownSkus = requestedSkus
                .Where(sku => !productsBySku.ContainsKey(sku))
                .ToList();

            if (unknownSkus.Count > 0)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Code = ErrorCodes.UnknownSku;
                response.Message =
                    $"Unknown SKU(s): {string.Join(", ", unknownSkus)}.";

                return response;
            }

            //FEFO plan
            var productIds = products.Select(p => p.Id).ToList();

            var stockLots = await _context.StockLots
                .Where(lot => productIds.Contains(lot.ProductId))
                .ToListAsync(ct);

            var lotsByProduct = stockLots
                .GroupBy(lot => lot.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var linePlans = new List<LineAllocationPlan>();

            foreach (var item in request.Items)
            {
                var product = productsBySku[item.Sku];

                var eligibleLots = lotsByProduct.TryGetValue(product.Id, out var lots)
                    ? lots
                        .Where(lot => lot.QuantityOnHand > 0 && lot.ExpiresOn >= today)
                        .OrderBy(lot => lot.ExpiresOn)
                        .ThenBy(lot => lot.Id)
                        .ToList()
                    : [];

                var remaining = item.Quantity;
                var allocations = new List<(StockLot Lot, int Quantity)>();

                foreach (var lot in eligibleLots)
                {
                    if (remaining <= 0) break;

                    var take = Math.Min(remaining, lot.QuantityOnHand);
                    allocations.Add((lot, take));
                    remaining -= take;
                }

                if (remaining > 0)
                {
                    var eligibleTotal = eligibleLots.Sum(lot => lot.QuantityOnHand);

                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Code = ErrorCodes.InsufficientStock;
                    response.Message =
                        $"{item.Sku} requires {item.Quantity} units but only {eligibleTotal} eligible units are available.";
                    return response;
                }

                linePlans.Add(new LineAllocationPlan(item, product, allocations));
            }

            // Begin Transaction
            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            var order = new Order
            {
                OrderNumber = request.OrderNumber,
                Status = OrderStatus.Allocated,
                CreatedAtUtc = DateTime.UtcNow,
            };

            var lineResponses = new List<OrderLineResponse>();

            foreach (var plan in linePlans)
            {
                var orderLine = new OrderLine
                {
                    ProductId = plan.Product.Id,
                    RequestedQuantity = plan.Item.Quantity,
                };

                var allocationResponses = new List<AllocationResponse>();

                foreach (var (lot, quantity) in plan.Allocations)
                {
                    orderLine.Allocations.Add(new Allocation
                    {
                        StockLotId = lot.Id,
                        Quantity = quantity,
                    });

                    lot.QuantityOnHand -= quantity;

                    allocationResponses.Add(new AllocationResponse
                    {
                        BatchCode = lot.BatchCode,
                        Quantity = quantity,
                    });
                }

                order.OrderLines.Add(orderLine);

                lineResponses.Add(new OrderLineResponse
                {
                    Sku = plan.Item.Sku,
                    RequestedQuantity = plan.Item.Quantity,
                    Allocations = allocationResponses,
                });
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            response.StatusCode = StatusCodes.Status201Created;
            response.Message = "Order created successfully.";
            response.Data = new OrderResponse
            {
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                CreatedAtUtc = order.CreatedAtUtc,
                CancelledAtUtc = order.CancelledAtUtc,
                Lines = lineResponses,
            };
            return response;

        }

        public async Task<GenericResponse<OrderResponse>> CancelOrderAsync(string orderNumber, CancellationToken ct)
        {
            var response = new GenericResponse<OrderResponse>();

            var order = await _context.Orders
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Allocations)
                        .ThenInclude(a => a.StockLot)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);

            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Code = ErrorCodes.OrderNotFound;
                response.Message = $"Order '{orderNumber}' was not found.";
                return response;
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                // Idempotent (scenario G).
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Order is already cancelled.";
                response.Data = BuildOrderResponse(order);
                return response;
            }

            await using var tx = await _context.Database.BeginTransactionAsync(ct);

            // Restore every lot by the exact quantity originally allocated — the Allocation
            foreach (var orderLine in order.OrderLines)
            {
                foreach (var allocation in orderLine.Allocations)
                {
                    allocation.StockLot.QuantityOnHand += allocation.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Order cancelled successfully.";
            response.Data = BuildOrderResponse(order);
            return response;
        }

        public async Task<GenericResponse<OrderResponse>> GetOrderAsync(string orderNumber, CancellationToken ct)
        {
            var response = new GenericResponse<OrderResponse>();

            var order = await _context.Orders
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Product)
                .Include(o => o.OrderLines)
                    .ThenInclude(ol => ol.Allocations)
                        .ThenInclude(a => a.StockLot)
                .FirstOrDefaultAsync(
                    o => o.OrderNumber == orderNumber,
                    ct);

            if (order is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Code = ErrorCodes.OrderNotFound;
                response.Message =
                    $"Order '{orderNumber}' was not found.";

                return response;
            }

            var orderResponse = new OrderResponse
            {
                OrderNumber = order.OrderNumber,
                Status = order.Status.ToString(),
                CreatedAtUtc = order.CreatedAtUtc,
                CancelledAtUtc = order.CancelledAtUtc,

                Lines = order.OrderLines
                    .Select(line => new OrderLineResponse
                    {
                        Sku = line.Product.Sku,
                        RequestedQuantity = line.RequestedQuantity,

                        Allocations = line.Allocations
                            .Select(allocation => new AllocationResponse
                            {
                                BatchCode = allocation.StockLot.BatchCode,
                                Quantity = allocation.Quantity
                            })
                            .ToList()
                    })
                    .ToList()
            };

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Order retrieved successfully.";
            response.Data = orderResponse;

            return response;
        }

        public async Task<GenericResponse<StockResponse>> GetStockAsync(string sku, CancellationToken ct)
        {
            var response = new GenericResponse<StockResponse>();

            var product = await _context.Products
                .Include(product => product.StockLots)
                .FirstOrDefaultAsync(
                    product => product.Sku == sku,
                    ct);

            if (product is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Code = ErrorCodes.UnknownSku;
                response.Message =
                    $"Product with SKU '{sku}' was not found.";

                return response;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var totalEligibleQuantity = product.StockLots
                .Where(lot =>
                    lot.QuantityOnHand > 0 &&
                    lot.ExpiresOn >= today)
                .Sum(lot => lot.QuantityOnHand);

            response.StatusCode = StatusCodes.Status200OK;
            response.Message = "Stock retrieved successfully.";

            response.Data = new StockResponse
            {
                Sku = product.Sku,
                Name = product.Name,

                Lots = product.StockLots
                    .OrderBy(lot => lot.ExpiresOn)
                    .ThenBy(lot => lot.Id)
                    .Select(lot => new StockLotResponse
                    {
                        BatchCode = lot.BatchCode,
                        QuantityOnHand = lot.QuantityOnHand,
                        ExpiresOn = lot.ExpiresOn
                    })
                    .ToList(),

                TotalEligibleQuantity = totalEligibleQuantity
            };

            return response;
        }
        private static OrderResponse BuildOrderResponse(Order order) => new()
        {
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            CreatedAtUtc = order.CreatedAtUtc,
            CancelledAtUtc = order.CancelledAtUtc,
            Lines = order.OrderLines.Select(ol => new OrderLineResponse
            {
                Sku = ol.Product.Sku,
                RequestedQuantity = ol.RequestedQuantity,
                Allocations = ol.Allocations.Select(a => new AllocationResponse
                {
                    BatchCode = a.StockLot.BatchCode,
                    Quantity = a.Quantity,
                }).ToList(),
            }).ToList(),
        };

        private sealed record LineAllocationPlan(
            CreateOrderItemRequest Item,
            Product Product,
            List<(StockLot Lot, int Quantity)> Allocations);
    }
}
