using Core.Interfaces;
using System.Security.Claims;

namespace WebAPI.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public string? UserName { get; }
    public string? Role { get; }


    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            UserId = Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
            UserName = user.Identity.Name;
            Role=user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
