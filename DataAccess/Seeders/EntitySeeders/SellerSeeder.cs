using Core.Constants;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class SellerSeeder
{
    public static async Task SeedSellersAsync(ApplicationDbContext dbContext)
    {
        if (await dbContext.Sellers.AnyAsync())
            return;

        var users = await dbContext.Users.ToListAsync();
        var plans = await dbContext.SubscriptionPlans.ToListAsync();

        var basicPlan = plans.First(p => p.Name == CustomPlans.Basic);
        var proPlan = plans.First(p => p.Name == CustomPlans.Pro);
        var enterprisePlan = plans.First(p => p.Name == CustomPlans.Enterprise);

        var sellers = new List<Seller>
        {
            // Active Seller - Secil Seleci(Enterprise Plan)
            new Seller
            {
                Id = users.FirstOrDefault(u => u.Email == "secil.seleci@gmail.com")!.Id,
                FirstName = "Secil",
                LastName = "Seleci",
                Phone = "05534102506",
                SubscriptionPlanId = enterprisePlan.Id,
                CreatedBy = "Seeder",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ModifiedBy ="Seeder",
                IsActive = true
            },

            // Inactive Seller - Ali Demir (Pro Plan)
            new Seller
            {
                Id = users.FirstOrDefault(u => u.Email == "alidemir@gmail.com")!.Id,
                FirstName = "Ali",
                LastName = "Demir",
                Phone = "05555552506",
                SubscriptionPlanId = proPlan.Id,
                CreatedBy = "Seeder",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ModifiedBy ="Seeder",
                IsActive = true
            },

            // Deleted Seller - Ayşe Kaya (Basic Plan)
            new Seller
            {
                Id = users.FirstOrDefault(u => u.Email == "aysekaya@gmail.com")!.Id,
                FirstName = "Ayşe",
                LastName = "Kaya",
                Phone = "05534102555",
                SubscriptionPlanId = basicPlan.Id,
                CreatedBy = "Seeder",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ModifiedBy ="Seeder",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow,
                DeletedBy="Seeder"
            },

            // Pending Seller - Alper Sütçü (Basic Plan)
            new Seller
            {
                Id = users.FirstOrDefault(u => u.Email == "alper@gmail.com")!.Id,
                FirstName = "Alper",
                LastName = "Sütçü",
                Phone = "05551234567",
                SubscriptionPlanId = basicPlan.Id,
                CreatedBy = "Seeder",
                CreatedAt = DateTime.UtcNow,
                IsActive = false
            }
        };

        await dbContext.Sellers.AddRangeAsync(sellers);
        await dbContext.SaveChangesAsync();
    }
}
