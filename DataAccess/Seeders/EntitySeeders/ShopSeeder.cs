using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class ShopSeeder
{
    public static async Task SeedShopsAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.Shops.AnyAsync()) return;

        var shopConfigs = new List<(string Email, string ShopName, string Status)>
        {
            ("secil.seleci@gmail.com", "Secil's Shop", "Active"),
            ("alidemir@gmail.com", "Ali's Shop", "Active"),
            ("aysekaya@gmail.com", "Ayşe's Shop", "Deleted"),
            ("alper@gmail.com", "Alper's Shop", "Pending")
        };

        foreach (var (email, shopName, status) in shopConfigs)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) continue;

            var seller = await dbContext.Sellers.FindAsync(user.Id);
            if (seller == null) continue;

            var now = DateTime.UtcNow;

            var shop = new Shop
            {
                Name = shopName,
                SellerId = seller.Id,
                Address = "Edirne, Turkey",
                TaxNumber = Guid.NewGuid().ToString().Substring(0, 10),
                CreatedAt = now,
                CreatedBy = seller.FirstName + " " + seller.LastName,
                ModifiedBy = "Seeder"
            };

            switch (status)
            {
                case "Active":
                    shop.IsActive = true;
                    break;

                case "Inactive":
                    shop.IsActive = false;
                    shop.UpdatedAt = now;
                    break;

                case "Deleted":
                    shop.IsActive = false;
                    shop.IsDeleted = true;
                    shop.UpdatedAt = now;
                    shop.DeletedAt = now;
                    shop.DeletedBy = "Seeder";
                    break;

                case "Pending":
                    shop.IsActive = false;
                    break;
            }

            await dbContext.Shops.AddAsync(shop);
        }

        await dbContext.SaveChangesAsync();
    }
}
