namespace Models.DTOs.Product;

public class ProductBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
    public Guid CategoryId { get; set; }

}
