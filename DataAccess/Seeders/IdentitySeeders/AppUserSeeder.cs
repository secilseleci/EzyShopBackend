using Microsoft.AspNetCore.Identity;
using Models.Identity;

namespace DataAccess.Seeders.IdentitySeeders;

public static class AppUserSeeder
{
    public static async Task SeedUsersAsync(ApplicationDbContext dbContext, UserManager<AppUser> userManager)
    {
        if (!dbContext.Users.Any())
        {
            // ✅ Admin
            var adminEmail = "admin@ezyshop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "05550000000",
                };

                var result = await userManager.CreateAsync(newAdmin, "Admin.1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }


            // ✅ Seller1
            var seller1Email = "secil.seleci@gmail.com";
            if (await userManager.FindByEmailAsync(seller1Email) == null)
            {
                var seller = new AppUser
                {
                    UserName = "Secil Seleci",
                    Email = seller1Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05534102506",

                };
                var resultSeller1 = await userManager.CreateAsync(seller, "Seller.1");

                if (resultSeller1.Succeeded)
                {
                    await userManager.AddToRoleAsync(seller, "Seller");
                }
            }


            // ✅ Seller2
            var seller2Email = "alidemir@gmail.com";

            if (await userManager.FindByEmailAsync(seller2Email) == null)
            {
                var seller = new AppUser
                {
                    UserName = "Ali Demir",
                    Email = seller2Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05555552506",
                };
                var resultSeller2 = await userManager.CreateAsync(seller, "Seller.1");

                if (resultSeller2.Succeeded)
                {
                    await userManager.AddToRoleAsync(seller, "Seller");
                }
            }


            // ✅ Seller3
            var seller3Email = "aysekaya@gmail.com";
            if (await userManager.FindByEmailAsync(seller3Email) == null)
            {
                var seller = new AppUser
                {
                    UserName = "Ayşe Kaya",
                    Email = seller3Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05534102555",
                    IsDeleted = true
                };
                var resultSeller3 = await userManager.CreateAsync(seller, "Seller.1");

                if (resultSeller3.Succeeded)
                {
                    await userManager.AddToRoleAsync(seller, "Seller");
                }
            }
            // ✅ Seller4
            var seller4Email = "alper@gmail.com";
            if (await userManager.FindByEmailAsync(seller4Email) == null)
            {
                var seller = new AppUser
                {
                    UserName = "Alper Sütçü",
                    Email = seller4Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05534102555"
                   
                };
                var resultSeller4 = await userManager.CreateAsync(seller, "Seller.1");

                if (resultSeller4.Succeeded)
                {
                    await userManager.AddToRoleAsync(seller, "Seller");
                }
            }


            // ✅ Customer1
            var customer1Email = "derya.karaman.ezyshop@gmail.com";
            if (await userManager.FindByEmailAsync(customer1Email) == null)
            {
                var customer = new AppUser
                {
                    UserName = "Derya Karaman",
                    Email = customer1Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05551112233",
                };

                var resultCustomer1 = await userManager.CreateAsync(customer, "Customer.1");

                if (resultCustomer1.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }

            }

            // ✅ Customer2

            var customer2Email = "nur.biral.ezyshop@gmail.com";
            if (await userManager.FindByEmailAsync(customer2Email) == null)
            {
                var customer = new AppUser
                {
                    UserName = "Nur Biral",
                    Email = customer2Email,
                    EmailConfirmed = true,
                    PhoneNumber = "05551112244",
                };

                var resultCustomer2 = await userManager.CreateAsync(customer, "Customer.1");

                if (resultCustomer2.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }

            }
        }
    }
}