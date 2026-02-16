using DataAccess;

namespace Api;

public class Seeder(MusicDbContext ctx) : ISeeder
{
    public async Task Seed()
    {
        ctx.Database.EnsureCreated();

        SeedAdminUser();

        await ctx.SaveChangesAsync();
    }

    private void SeedAdminUser()
    {
        if (!ctx.Users.Any(u => u.Username == "admin"))
        {
            var admin = new User
            {
                Username = "admin",
                Password = "passwd123",
                Email = "admin@gmail.com"
            };

            ctx.Users.Add(admin);
        }
    }
}

public interface ISeeder
{
    Task Seed();
}