using Microsoft.EntityFrameworkCore;
using StockAllocation.Domain.Entities;

namespace StockAllocation.Application.Abstractions.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }

        DbSet<StockLot> StockLots { get; }

        DbSet<Order> Orders { get; }

        DbSet<OrderLine> OrderLines { get; }

        DbSet<Allocation> Allocations { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);  
    }
}
