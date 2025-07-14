using Models.ViewModels.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Product;

public class CreateProductViewModel : IImageViewModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string? Color { get; set; }

    [Required]
    [Range(1, 10000000)]
    public decimal Price { get; set; }

    [Required]
    public int Stock { get; set; } = 1;


    [Required(ErrorMessage = "Category required")]

    public Guid CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string FolderName { get; set; } = "product";
}
