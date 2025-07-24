namespace Models.DTOs.Shop;

public class ShopDetailsDto
{
    public Guid SellerId { get; set; }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SellerName { get; set; }
    public string Email { get; set; }
    public string TaxNumber { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}