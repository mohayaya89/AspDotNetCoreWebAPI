using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AspDotNetCoreWebAPI.Models
{
    // This factory is used by EF tools (dotnet ef) at design-time to create a DbContext
    public class DesignTimeShopContextFactory : IDesignTimeDbContextFactory<ShopContext>
    {
        public ShopContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShopContext>();
            // Use the same default connection as appsettings.json
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ShopDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new ShopContext(optionsBuilder.Options);
        }
    }
}
