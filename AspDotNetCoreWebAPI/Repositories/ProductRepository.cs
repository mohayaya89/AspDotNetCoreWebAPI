using AspDotNetCoreWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AspDotNetCoreWebAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopContext _db;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ShopContext db, ILogger<ProductRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogDebug("GetAllAsync: starting query for products.");

            var products = await _db.Products
                .Include(p => p.Category)
                .ToArrayAsync();

            _logger.LogInformation("GetAllAsync: retrieved {Count} products.", products.Length);
            return products;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            _logger.LogDebug("GetByIdAsync: fetching product with id {ProductId}.", id);

            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                _logger.LogWarning("GetByIdAsync: product with id {ProductId} not found.", id);
            }
            else
            {
                _logger.LogInformation("GetByIdAsync: found product with id {ProductId}.", id);
            }

            return product;
        }

        public Task AddAsync(Product product)
        {
            _logger.LogDebug("AddAsync: adding product Sku={Sku}, Name={Name}.", product.Sku, product.Name);
            _db.Products.Add(product);
            return Task.CompletedTask;
        }

        public void Update(Product product)
        {
            _logger.LogDebug("Update: updating product Id={ProductId}.", product.Id);
            _db.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _logger.LogDebug("Remove: removing product Id={ProductId}.", product.Id);
            _db.Products.Remove(product);
        }

        public async Task SaveChangesAsync()
        {
            _logger.LogDebug("SaveChangesAsync: saving changes to database.");
            try
            {
                await _db.SaveChangesAsync();
                _logger.LogInformation("SaveChangesAsync: changes saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveChangesAsync: error while saving changes.");
                throw;
            }
        }
    }
}