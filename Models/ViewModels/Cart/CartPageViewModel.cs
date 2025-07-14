using Models.DTOs.OrderItem;

namespace Models.ViewModels.Cart;
public class CartPageViewModel  
{ 
    public decimal TotalAmount { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];
}