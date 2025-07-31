namespace Models.ViewModels.Product;

public class ProductFilterViewModel
{ 
    public string? Name { get; set; }    
    public string? Color { get; set; }

    public string? CategoryName { get; set; }
    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; } = "name";
    public bool Descending { get; set; } = false;

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
