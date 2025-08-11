namespace Models.DTOs.Order;

public class CartFlatRow
{
    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = null!;

    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }

    // snapshot alanlar OrderItem’tan:
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Count { get; set; }
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
}
