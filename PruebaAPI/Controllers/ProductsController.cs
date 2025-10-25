using Microsoft.AspNetCore.Mvc;
using PruebaAPI.Models;
using PruebaAPI.Repositories;

namespace PruebaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting all products");
        var products = await _productRepository.GetAllAsync();
        return Ok(products);
    }

    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with id: {Id}", id);
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            _logger.LogWarning("Product with id: {Id} not found", id);
            return NotFound();
        }

        return Ok(product);
    }

    // GET: api/Products/search?name=laptop
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string name)
    {
        _logger.LogInformation("Searching products by name: {Name}", name);
        
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Search name cannot be empty");
        }

        var products = await _productRepository.SearchByNameAsync(name);
        return Ok(products);
    }

    // GET: api/Products/in-stock
    [HttpGet("in-stock")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsInStock()
    {
        _logger.LogInformation("Getting products in stock");
        var products = await _productRepository.GetProductsInStockAsync();
        return Ok(products);
    }

    // GET: api/Products/price-range?minPrice=10&maxPrice=100
    [HttpGet("price-range")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPriceRange(
        [FromQuery] decimal minPrice,
        [FromQuery] decimal maxPrice)
    {
        _logger.LogInformation("Getting products in price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);

        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            return BadRequest("Invalid price range");
        }

        var products = await _productRepository.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return Ok(products);
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _logger.LogInformation("Creating new product: {Name}", product.Name);
        
        product.CreatedAt = DateTime.UtcNow;
        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT: api/Products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("Product ID mismatch");
        }

        _logger.LogInformation("Updating product with id: {Id}", id);

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product with id: {Id} not found", id);
            return NotFound();
        }

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with id: {Id}", id);
        
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with id: {Id} not found", id);
            return NotFound();
        }

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();

        return NoContent();
    }
}
