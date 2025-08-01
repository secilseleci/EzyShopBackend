using static Models.Entities.Concrete.Seller;
namespace Models.DTOs.Seller;

public class SellerFilterDto
{
    public string? SearchTerm { get; set; }
    public SellerStatus? Status { get; set; }

    public string? SortBy { get; set; } = "createdDate";
    public bool Descending { get; set; } = true;

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
