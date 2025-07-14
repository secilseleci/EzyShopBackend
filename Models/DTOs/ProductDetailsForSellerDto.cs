namespace Models.DTOs;

public class ProductDetailsForSellerDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; } = 1;
    public string? Color { get; set; }
}