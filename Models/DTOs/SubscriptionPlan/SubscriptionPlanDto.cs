using System.ComponentModel.DataAnnotations;

namespace Models.DTOs.SubscriptionPlan;
public class SubscriptionPlanDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
    public decimal Price { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "MaxProducts must be greater than 0")]
    public int MaxProducts { get; set; }

    public bool IsActive { get; set; }
}
