using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;

public class Seller : BaseEntity
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required] 
    public string Phone { get; set; } = null!;


}