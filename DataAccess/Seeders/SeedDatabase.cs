using DataAccess.Seeders.EntitySeeders;
using DataAccess.Seeders.IdentitySeeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.Identity;

namespace DataAccess.SeedDatabase;

public static class SeedDatabase
{
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // ✅ Roles
        await RoleSeeder.SeedRolesAsync(dbContext, roleManager);

        // ✅ Users 
        await AppUserSeeder.SeedUsersAsync(dbContext, userManager);

        ////// ✅ Sellers  
        await SellerSeeder.SeedSellersAsync(dbContext);

        ////// ✅ Shops
        await ShopSeeder.SeedShopsAsync(dbContext);

        ////// ✅ Customers  
        await CustomerSeeder.SeedCustomersAsync(dbContext);
         
        //// ✅ Categories
        await CategorySeeder.SeedCategoriesAsync(dbContext);

        ////  ✅ Products
        await ProductSeeder.SeedProductsAsync(dbContext);

        // ✅ Orders
        //await OrderSeeder.SeedOrdersAsync(dbContext);

         
    }
}
