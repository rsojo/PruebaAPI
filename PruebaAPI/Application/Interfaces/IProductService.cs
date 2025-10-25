using PruebaAPI.Application.DTOs;

namespace PruebaAPI.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name);
    Task<IEnumerable<ProductDto>> GetProductsInStockAsync();
    Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
}
