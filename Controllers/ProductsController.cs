using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudApp.Data;
using CrudApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CrudApp.Controllers;

/// <summary>
/// Controller for managing products (CRUD operations)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all products from the database
    /// </summary>
    /// <returns>List of all products</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await _context.Products.ToListAsync();
            _logger.LogInformation("Retrieved {Count} products", products.Count);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "An error occurred while retrieving products");
        }
    }

    /// <summary>
    /// Retrieves a specific product by ID
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <returns>The product with the specified ID</returns>
    /// <response code="200">Returns the requested product</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound(new { message = $"Product with ID {id} was not found" });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">The product object to create</param>
    /// <returns>The newly created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid product data</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid product data provided");
                return BadRequest(ModelState);
            }

            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created with ID {ProductId}", product.Id);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">The unique identifier of the product to update</param>
    /// <param name="product">The updated product data</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">Invalid product data or ID mismatch</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        try
        {
            if (id != product.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {UrlId} does not match product ID {ProductId}", id, product.Id);
                return BadRequest(new { message = "The ID in the URL must match the ID in the request body." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid product data provided for update");
                return BadRequest(ModelState);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return NotFound(new { message = $"Product with ID {id} was not found" });
            }

            // Update the existing product with new values
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID {ProductId} updated successfully", id);

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound(new { message = $"Product with ID {id} was not found" });
            }
            else
            {
                _logger.LogError("Concurrency exception while updating product with ID {ProductId}", id);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    /// <summary>
    /// Deletes a product by ID
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                return NotFound(new { message = $"Product with ID {id} was not found" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product with ID {ProductId} deleted successfully", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
