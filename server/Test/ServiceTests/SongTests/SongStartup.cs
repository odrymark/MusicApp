using Api.Services.Song;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test.ServiceTests.SongTests;

public class SongStartup
{
    private static DbContainer? _container;

    public void ConfigureServices(IServiceCollection services)
    {
        if (_container == null)
        {
            _container = new DbContainer();
            var initTask = _container.InitializeAsync();
            if (!initTask.IsCompleted)
            {
                initTask.GetAwaiter().GetResult();
            }
        }

        services.AddSingleton(_container);

        services.AddDbContext<MusicDbContext>(options =>
            options.UseNpgsql(_container.Container.GetConnectionString()));

        services.AddScoped<ISongService, SongService>();
    }
}