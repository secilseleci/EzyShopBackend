namespace Models.DTOs.Product;

public class ProductListForSellerDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string CategoryName { get; set; }
    public Guid CategoryId { get; set; }  
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
