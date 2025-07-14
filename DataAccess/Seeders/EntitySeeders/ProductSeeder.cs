using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class ProductSeeder
{
    public static async Task SeedProductsAsync(ApplicationDbContext dbContext)
    {
        var sellerMail1 = "secil.seleci@gmail.com";
        var sellerMail2 = "alidemir@gmail.com";

        var user1 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == sellerMail1);
        var user2 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == sellerMail2);

        if (user1 == null || user2 == null)
            return;

        var seller1 = await dbContext.Sellers.FirstOrDefaultAsync(s => s.Id == user1.Id);
        var seller2 = await dbContext.Sellers.FirstOrDefaultAsync(s => s.Id == user2.Id);

        if (seller1 == null || seller2 == null)
            return;

        var shop1 = await dbContext.Shops.FirstOrDefaultAsync(s => s.SellerId == seller1.Id && !s.IsDeleted);
        var shop2 = await dbContext.Shops.FirstOrDefaultAsync(s => s.SellerId == seller2.Id && !s.IsDeleted);

        if (shop1 == null || shop2 == null)
            return;

        var electronicsCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
        var booksCategory = await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Books");

        if (electronicsCategory == null || booksCategory == null)
            return;

        var products = new List<Product>();
        var random = new Random();

        // Secil için 5 ürün
        for (int i = 1; i <= 5; i++)
        {
            products.Add(new Product
            {
                Name = $"ElectronicProduct {i}",
                CategoryId = electronicsCategory.Id,
                ShopId = shop1.Id,
                Price = random.Next(100, 501),
                Stock = 5,
                ImageUrl = $"/images/product/e{i}.jpg",
                IsActive = true,
                CreatedBy = seller1.FirstName + " " + seller1.LastName,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Ali için 5 ürün
        for (int i = 1; i <= 5; i++)
        {
            products.Add(new Product
            {
                Name = $"BookProduct {i}",
                CategoryId = booksCategory.Id,
                ShopId = shop2.Id,
                Price = random.Next(100, 501),
                Stock = 5,
                ImageUrl = $"/images/product/{i}.jpg",
                IsActive = true,
                CreatedBy = seller2.FirstName + " " + seller2.LastName,
                CreatedAt = DateTime.UtcNow
            });
        }
        if (!dbContext.Products.Any())
        {
            await dbContext.Products.AddRangeAsync(products);
        }
        await dbContext.SaveChangesAsync();
    }
}
