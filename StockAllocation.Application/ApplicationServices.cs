using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockAllocation.Application.Abstractions;
using StockAllocation.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockAllocation.Application
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IStockAllocationService, StockAllocationService>();
            return services;
        }
    }
}
