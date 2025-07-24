namespace Models.DTOs.Product;

public class ProductFilterViewModel
{ 
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Color { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
