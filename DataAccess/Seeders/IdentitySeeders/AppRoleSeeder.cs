using Core.Constants;
using Microsoft.AspNetCore.Identity;
using Models.Identity;

namespace DataAccess.Seeders.IdentitySeeders;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(ApplicationDbContext dbContext, RoleManager<AppRole> roleManager)
    {
        if (!dbContext.Roles.Any())
        {
            var roles = CustomRoles.All;

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole { Name = role });
                }
            }
        }
    }
}