using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Interfaces;
using PruebaAPI.Domain.Entities;
using PruebaAPI.Domain.Interfaces;

namespace PruebaAPI.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        _logger.LogInformation("Getting all products from service layer");
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Getting product with id: {Id} from service layer", id);
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name)
    {
        _logger.LogInformation("Searching products by name: {Name}", name);
        var products = await _unitOfWork.Products.SearchByNameAsync(name);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsInStockAsync()
    {
        _logger.LogInformation("Getting products in stock");
        var products = await _unitOfWork.Products.GetProductsInStockAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        _logger.LogInformation("Getting products in price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
        var products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        _logger.LogInformation("Creating new product: {Name}", createProductDto.Name);
        
        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        _logger.LogInformation("Updating product with id: {Id}", id);

        var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product with id: {Id} not found", id);
            return false;
        }

        existingProduct.Name = updateProductDto.Name;
        existingProduct.Description = updateProductDto.Description;
        existingProduct.Price = updateProductDto.Price;
        existingProduct.Stock = updateProductDto.Stock;

        await _unitOfWork.Products.UpdateAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("Deleting product with id: {Id}", id);

        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with id: {Id} not found", id);
            return false;
        }

        await _unitOfWork.Products.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedAt = product.CreatedAt
        };
    }
}
