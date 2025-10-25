using PruebaAPI.Models;

namespace PruebaAPI.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetProductsInStockAsync();
    Task<IEnumerable<Product>> SearchByNameAsync(string name);
}
