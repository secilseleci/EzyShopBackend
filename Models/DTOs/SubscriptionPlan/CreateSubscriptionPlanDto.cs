using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.SubscriptionPlan;

public class CreateSubscriptionPlanDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "MaxProducts must be greater than 0")]
    public int? MaxProducts { get; set; }

    public bool IsActive { get; set; }         
}
