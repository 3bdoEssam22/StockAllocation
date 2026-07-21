
using StockAllocation.API.Extension;
using StockAllocation.Infrastructure;

namespace StockAllocation.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddInfrastructure(builder.Configuration);


            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.

            await app.MigrateDatabaseAsync();


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
