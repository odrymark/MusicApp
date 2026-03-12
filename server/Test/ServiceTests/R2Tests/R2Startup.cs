using Api.Services.R2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Test.ServiceTests.R2Tests;

public class R2Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var r2Mock = Substitute.For<IR2Service>();
        r2Mock.UploadSongStorage(Arg.Any<IFormFile>())
            .Returns("uploaded_file.mp3");
        r2Mock.GenerateSignedUrl(Arg.Any<string>(), Arg.Any<int>())
            .Returns("https://signed-url.example.com/file.mp3");

        services.AddSingleton(r2Mock);
    }
}