using Models.DTOs.OrderItem;
using Models.Entities.Concrete;

namespace Models.ViewModels.Cart;
public class ConfirmViewModel
{
    public Guid OrderId { get; set; }
   
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = [];

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;
}