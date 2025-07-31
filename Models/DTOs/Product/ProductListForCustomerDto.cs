namespace Models.DTOs.Product;

public class ProductListForCustomerDto
{
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string ShopName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; } = 1;
    public string? Color { get; set; }
}
