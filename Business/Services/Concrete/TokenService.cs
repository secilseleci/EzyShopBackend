using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Business.Services.Concrete;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string CreateToken(AppUser user, IList<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }; 
        
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        var key = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("JWT key is not configured.");

        var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
 
