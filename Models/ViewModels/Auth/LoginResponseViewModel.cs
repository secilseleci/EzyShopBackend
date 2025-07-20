namespace Models.ViewModels.Auth;

public class LoginResponseViewModel
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IList<string> Roles { get; set; } = new List<string>();
}
