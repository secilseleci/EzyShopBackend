namespace Models.DTOs.Order;
public class CartItemDto
{
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Count { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }            
    public string? ImageUrl { get; set; }
    public string? Color { get; set; }
}
