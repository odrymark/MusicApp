using Api;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MusicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISeeder, Seeder>();
builder.Services.AddScoped<MainService>();
builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
    await seeder.Seed();
}

app.UseOpenApi();
app.UseSwaggerUi();
app.MapControllers();
app.Run();