using System.ComponentModel.DataAnnotations;

namespace Models.Entities.Concrete;

public class Shop : BaseEntity
{
    public Guid SellerId { get; set; }


    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;


    [Required]
    [StringLength(500)]
    public string Address { get; set; } = null!;


    [Required]
    public string TaxNumber { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = [];

}
