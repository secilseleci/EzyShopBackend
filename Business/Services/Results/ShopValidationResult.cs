using Models.Entities.Concrete;
using Models.Identity;

namespace Business.Services.Results;

public class ShopValidationResult
{
    public Shop Shop { get; set; }
    public Seller Seller { get; set; }
    public AppUser User { get; set; }
}
