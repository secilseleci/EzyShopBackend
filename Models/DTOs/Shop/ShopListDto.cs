namespace Models.DTOs;

public class ShopListDto
{
    public Guid SellerId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SellerName { get; set; }
}