using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;

public class Customer : BaseEntity
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = [];
}
