using Core.Constants;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class SubscriptionPlanSeeder
{
    public static async Task SeedSubscriptionPlansAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.SubscriptionPlans.AnyAsync()) return;

        var now = DateTime.UtcNow;

        var plans = new List<SubscriptionPlan>
        {
            new SubscriptionPlan
            {
                Name = CustomPlans.Basic,
                Price = 400m,
                MaxProducts = 50,
                CreatedAt = now,
                CreatedBy = "Seeder",
                IsActive = true
            },
            new SubscriptionPlan
            {
                Name = CustomPlans.Pro,
                Price = 900m,
                MaxProducts = 200,
                CreatedAt = now,
                CreatedBy = "Seeder",
                IsActive = true
            },
            new SubscriptionPlan
            {
                Name = CustomPlans.Enterprise,
                Price = 1500m,
                MaxProducts = null, // sınırsız ürün
                CreatedAt = now,
                CreatedBy = "Seeder",
                IsActive = true
            }
        };

        await dbContext.SubscriptionPlans.AddRangeAsync(plans);
        await dbContext.SaveChangesAsync();
    }
}
