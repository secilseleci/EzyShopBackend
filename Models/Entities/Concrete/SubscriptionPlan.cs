using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;
public class SubscriptionPlan : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    public int? MaxProducts { get; set; }
 }
