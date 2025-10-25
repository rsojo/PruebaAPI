using Microsoft.AspNetCore.Mvc;
using PruebaAPI.Application.DTOs;
using PruebaAPI.Application.Interfaces;

namespace PruebaAPI.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        _logger.LogInformation("API: Getting all products");
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        _logger.LogInformation("API: Getting product with id: {Id}", id);
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Search products by name
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string name)
    {
        _logger.LogInformation("API: Searching products by name: {Name}", name);
        
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Search name cannot be empty" });
        }

        var products = await _productService.SearchProductsByNameAsync(name);
        return Ok(products);
    }

    /// <summary>
    /// Get products in stock
    /// </summary>
    [HttpGet("in-stock")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsInStock()
    {
        _logger.LogInformation("API: Getting products in stock");
        var products = await _productService.GetProductsInStockAsync();
        return Ok(products);
    }

    /// <summary>
    /// Get products by price range
    /// </summary>
    [HttpGet("price-range")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange(
        [FromQuery] decimal minPrice,
        [FromQuery] decimal maxPrice)
    {
        _logger.LogInformation("API: Getting products in price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);

        if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
        {
            return BadRequest(new { message = "Invalid price range" });
        }

        var products = await _productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        _logger.LogInformation("API: Creating new product: {Name}", createProductDto.Name);
        
        var product = await _productService.CreateProductAsync(createProductDto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        if (id != updateProductDto.Id)
        {
            return BadRequest(new { message = "Product ID mismatch" });
        }

        _logger.LogInformation("API: Updating product with id: {Id}", id);

        var result = await _productService.UpdateProductAsync(id, updateProductDto);
        if (!result)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("API: Deleting product with id: {Id}", id);
        
        var result = await _productService.DeleteProductAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Product with ID {id} not found" });
        }

        return NoContent();
    }
}
