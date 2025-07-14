using Models.Entities.Concrete;

namespace DataAccess.Seeders.EntitySeeders;

public static class CategorySeeder
{
    public static async Task SeedCategoriesAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange(new List<Category>
            {
                new Category {
                    Name = "Electronics" ,
                    ImageUrl= "/images/category/electronics.jpg",
                    CreatedBy="Seeder",
                    CreatedAt=DateTime.UtcNow
                },
                new Category {
                    Name = "Books",
                    ImageUrl="/images/category/books.jpg",
                    CreatedBy="Seeder",
                    CreatedAt=DateTime.UtcNow},
                new Category {
                    Name = "Fashion",
                    ImageUrl = "/images/category/fashion.jpg",
                    CreatedBy="Seeder",                                    
                    CreatedAt=DateTime.UtcNow},
            });

            await dbContext.SaveChangesAsync();
        }
    }
}
