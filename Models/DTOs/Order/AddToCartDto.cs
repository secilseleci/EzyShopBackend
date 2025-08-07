using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.Order;

public class AddToCartDto
{
    public Guid ProductId { get; set; }

    [Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")]
    public int Count { get; set; } = 1;
}
