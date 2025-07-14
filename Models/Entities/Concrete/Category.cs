using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;

public class Category : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = [];
}
