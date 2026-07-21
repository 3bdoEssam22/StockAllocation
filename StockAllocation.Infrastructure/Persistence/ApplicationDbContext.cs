using Microsoft.EntityFrameworkCore;
using StockAllocation.Application.Abstractions;
using StockAllocation.Domain.Entities;
using System.Reflection;

namespace StockAllocation.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                    : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<StockLot> StockLots { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderLine> OrderLines { get; set; } = null!;

        public DbSet<Allocation> Allocations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
