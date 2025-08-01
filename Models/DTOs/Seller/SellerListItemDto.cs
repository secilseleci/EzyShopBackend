using static Models.Entities.Concrete.Seller;

namespace Models.DTOs.Seller;

public class SellerListItemDto
{
    public Guid SellerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public SellerStatus Status { get; set; }
    public string ShopName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}
