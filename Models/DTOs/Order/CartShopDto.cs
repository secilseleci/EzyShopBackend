namespace Models.DTOs.Order;
public class CartShopDto
{
    public Guid ShopId { get; set; }
    public string ShopName { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}
