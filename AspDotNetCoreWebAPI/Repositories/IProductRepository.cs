using AspDotNetCoreWebAPI.Models;

namespace AspDotNetCoreWebAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        void Update(Product product);
        void Remove(Product product);
        Task SaveChangesAsync();
    }
}