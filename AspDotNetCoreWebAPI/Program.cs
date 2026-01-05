using AspDotNetCoreWebAPI.Models;
using AspDotNetCoreWebAPI.Models.Dto;
using AspDotNetCoreWebAPI.Models.HealthChecks;
using AspDotNetCoreWebAPI.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog from configuration (appsettings)
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddControllers();

        // Health checks
        builder.Services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        // Register repository
        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
      
        builder.Services.AddDbContext<ShopContext>(options =>
            options.UseSqlServer(connectionString)
        );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging(); // structured request logging

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ShopContext>();
            // Apply any pending migrations and create the database if it does not exist
            await db.Database.MigrateAsync();
        }

        // Map health endpoints
        app.MapHealthChecks("/health");
        // Readiness endpoint (same in this simple example)
        app.MapHealthChecks("/health/ready");

        app.MapGet("/products", async (ShopContext _context) =>
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToArrayAsync();

            var dtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.IsAvailable,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            });

            return Results.Ok(dtos);
        });

        app.MapGet("/products/{id}", async (int id, ShopContext _context) =>
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return Results.NotFound();

            var dto = new ProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };

            return Results.Ok(dto);
        });

        app.Run();
    }
}
