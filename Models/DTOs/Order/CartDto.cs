namespace Models.DTOs.Order;

public class CartDto
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalItemCount { get; set; }
    public int DistinctShopCount { get; set; }
    public List<CartShopDto> Shops { get; set; } = [];
}
