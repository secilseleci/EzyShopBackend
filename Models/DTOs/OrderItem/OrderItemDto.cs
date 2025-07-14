namespace Models.DTOs.OrderItem;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal ProductPrice { get; set; }
    public int Count { get; set; }
    public decimal TotalPrice => Count * ProductPrice;
    public string? Color { get; set; }
    public string? ImageUrl { get; set; }
    public string ShopName { get; set; } = null!;
}
