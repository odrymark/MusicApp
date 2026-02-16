using Api.Services;
using DataAccess;

namespace Api;

public class Seeder(MusicDbContext ctx) : ISeeder
{
    public async Task Seed()
    {
        await ctx.Database.EnsureCreatedAsync();

        SeedAdminUser();

        await ctx.SaveChangesAsync();
    }

    private void SeedAdminUser()
    {
        var passwd = new PasswordService().HashPassword("passwd123");
        
        if (!ctx.Users.Any(u => u.username == "admin"))
        {
            var admin = new User
            {
                username = "admin",
                password = passwd,
                email = "admin@gmail.com",
                isAdmin = true
            };

            ctx.Users.Add(admin);
        }
    }
}

public interface ISeeder
{
    Task Seed();
}