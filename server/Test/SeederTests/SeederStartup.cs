using Api;
using Api.Services.Password;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.DependencyInjection;
 
namespace Test.SeederTests;
 
public class SeederStartup
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
        
        var passwordMock = Substitute.For<IPasswordService>();
        passwordMock.HashPassword(Arg.Any<string>())
            .Returns(ci => "HASH_" + ci.Arg<string>());
 
        services.AddSingleton(passwordMock);
    }
}