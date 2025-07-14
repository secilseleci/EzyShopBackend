namespace Models.DTOs.OrderItem;

public class UpdateOrderItemCountDto
{
    public Guid OrderItemId { get; set; }
    public int Delta { get; set; }
}
