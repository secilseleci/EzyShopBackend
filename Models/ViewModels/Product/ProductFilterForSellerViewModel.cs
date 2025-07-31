namespace Models.ViewModels.Product;

public class ProductFilterForSellerViewModel
{
    public bool? IsActive { get; set; }  
    public Guid? CategoryId { get; set; }
    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; } = "name";  
    public bool Descending { get; set; } = false;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
