using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class CustomerSeeder
{
    public static async Task SeedCustomersAsync(ApplicationDbContext dbContext)
    {
        var customerMails = new[]
        {
            "derya.karaman.ezyshop@gmail.com",
            "nur.biral.ezyshop@gmail.com"
        };

        foreach (var email in customerMails)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && !await dbContext.Customers.AnyAsync(c => c.Id == user.Id))
            {
                var firstName = user.UserName.Split(" ")[0];
                var lastName = user.UserName.Split(" ").Length > 1 ? user.UserName.Split(" ")[1] : "";

                var customer = new Customer
                {
                    Id = user.Id,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = user.PhoneNumber,
                    IsActive = true,
                    Address="Default Address",
                    CreatedBy = "Seeder",
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Customers.AddAsync(customer);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
