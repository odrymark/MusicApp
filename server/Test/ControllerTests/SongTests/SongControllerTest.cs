using Api.Controllers;
using Api.DTOs.Request;
using Api.DTOs.Response;
using Api.Services.Song;
using Api.Services.R2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Test.ControllerTests.SongTests;

public class SongControllerTests
{
    private readonly ISongService _mockSongService;
    private readonly IR2Service _mockR2Service;
    private readonly SongController _controller;
    private readonly SongControllerStartup _startup;

    public SongControllerTests()
    {
        var services = new ServiceCollection();
        _startup = new SongControllerStartup();
        _startup.ConfigureServices(services);

        var provider = services.BuildServiceProvider();
        _mockSongService = provider.GetRequiredService<ISongService>();
        _mockR2Service = provider.GetRequiredService<IR2Service>();
        _controller = _startup.GetController(provider);
    }

    [Fact]
    public async Task UploadSong_Calls_R2_For_Both_Song_And_Image()
    {
        var userId = Guid.NewGuid();
        _startup.SetupUserClaims(_controller, userId);

        var mockSongFile = Substitute.For<IFormFile>();
        var mockImgFile = Substitute.For<IFormFile>();
        
        var dto = new UploadSongReqDto 
        { 
            title = "New Track", 
            file = mockSongFile, 
            image = mockImgFile, 
            artist = "AI Artist",
            isPublic = true 
        };

        _mockR2Service.UploadSongStorage(mockSongFile).Returns("song-key-123");
        _mockR2Service.UploadImageStorage(mockImgFile).Returns("img-key-456");

        var result = await _controller.UploadSong(dto);

        Assert.IsType<OkResult>(result);
        await _mockR2Service.Received(1).UploadSongStorage(mockSongFile);
        await _mockR2Service.Received(1).UploadImageStorage(mockImgFile);
        await _mockSongService.Received(1).CreateSong(userId, dto.title, "song-key-123", dto.artist, dto.isPublic, "img-key-456");
    }

    [Fact]
    public async Task GetUserSongs_Returns_Songs_For_Logged_In_User()
    {
        var userId = Guid.NewGuid();
        _startup.SetupUserClaims(_controller, userId);
        _mockSongService.GetUserSongs(userId).Returns(new List<SongResDto>());

        var result = await _controller.GetUserSongs();

        Assert.IsType<OkObjectResult>(result);
        await _mockSongService.Received(1).GetUserSongs(userId);
    }

    [Fact]
    public void GetSignedUrl_Returns_Ok_With_Url()
    {
        var key = "some-file-key";
        _mockR2Service.GenerateSignedUrl(key).Returns("https://signed-url.com");

        var result = _controller.GetSignedUrl(key);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("https://signed-url.com", okResult.Value);
    }

    [Fact]
    public async Task EditSong_Deletes_Old_Image_Only_If_New_One_Provided()
    {
        var userId = Guid.NewGuid();
        _startup.SetupUserClaims(_controller, userId);

        var dto = new SongEditReqDto 
        { 
            id = Guid.NewGuid(),
            title = "Updated Title",
            image = Substitute.For<IFormFile>(),
            prevImgKey = "old-image-key",
            artist = "TestArtist",
            isPublic = true
        };

        await _controller.EditSong(dto);

        await _mockR2Service.Received(1).DeleteFile("old-image-key");
        await _mockSongService.Received(1).EditSong(userId, dto.id, dto.title, Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string>());
    }
}