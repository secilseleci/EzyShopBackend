using Microsoft.AspNetCore.Identity;

namespace Models.Identity;

public class AppUser : IdentityUser<Guid>
{
    public DateTime? LastLoginDate { get; set; }
    public bool IsDeleted { get; set; }
}
