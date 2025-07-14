using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;
public class Order : BaseEntity
{
    public Order()
    {
        Status = OrderStatus.InCart;
    }
    
    public Guid CustomerId { get; set; }

    public string? OrderNumber { get; set; }  

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    public OrderStatus Status { get; set; } 

    public string? ShippingTrackingNumber { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = [];

}

public enum PaymentMethod
{
    CreditCard = 1,
    BankTransfer = 2,
    CashOnDelivery = 3
}
public enum OrderStatus
{
    InCart = 0,
    Pending = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

