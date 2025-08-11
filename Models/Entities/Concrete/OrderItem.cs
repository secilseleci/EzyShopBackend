using System.ComponentModel.DataAnnotations;
 
namespace Models.Entities.Concrete;

public class OrderItem : BaseEntity
{
    public OrderItem()
    {
        Status =OrderItemStatus.InCart;
        IsActive = true;
    }

    public Guid OrderId { get; set; }


    public Guid ProductId { get; set; }

     
    [Required]
    public string ProductName { get; set; } = null!;


    [Required]
    public decimal ProductPrice { get; set; }

    [Required]
    public OrderItemStatus Status { get; set; }
   
    [Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")]
    public int Count { get; set; } = 1;

    public decimal TotalPrice => Count * ProductPrice;
    public string? Color { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } = string.Empty;

   
    public enum OrderItemStatus
    {
        InCart = 0,
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}