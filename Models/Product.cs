using System.ComponentModel.DataAnnotations;

namespace CrudApp.Models;

/// <summary>
/// Represents a product in the system
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the product (required, max 200 characters)
    /// </summary>
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the product (max 1000 characters)
    /// </summary>
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Price of the product (must be greater than 0)
    /// </summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// Category of the product (max 100 characters)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Date and time when the product was created (automatically set)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
