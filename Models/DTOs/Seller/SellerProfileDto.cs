namespace Models.DTOs.Seller;

public class SellerProfileDto
{
    public Guid SellerId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string ShopName { get; set; } = null!;
    public string TaxNumber { get; set; } = null!;

    public DateTime CreatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
