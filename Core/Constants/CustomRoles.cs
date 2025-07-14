namespace Core.Constants;

public class CustomRoles
{
    public const string Admin = nameof(Admin);
    public const string Seller = nameof(Seller);
    public const string Customer = nameof(Customer);
    public static readonly string[] All = { Admin, Seller, Customer };
}
