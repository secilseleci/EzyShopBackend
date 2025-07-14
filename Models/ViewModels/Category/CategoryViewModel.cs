using Models.ViewModels.Abstract;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Category;

public class CategoryViewModel : IImageViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Category name must be between 3-30 characters")]
    public string Name { get; set; } = null!;

    [DisplayName("Image")]
    public string? ImageUrl { get; set; }

    public string FolderName { get; set; } = "category";
}
