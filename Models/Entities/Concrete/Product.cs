using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;

public class Product : BaseEntity
{
    public Product()
    {
        IsActive = true;
    }
    
    public Guid CategoryId { get; set; }

    public Guid ShopId { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; } = 1;
  
    public string? ImageUrl { get; set; }

    public string? Color { get; set; } 

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}
