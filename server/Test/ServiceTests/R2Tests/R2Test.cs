using Api.Services.R2;
using DataAccess;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit.DependencyInjection;

namespace Test.ServiceTests.R2Tests;

[Startup(typeof(R2Startup))]
public class R2ServiceTests
{
    private readonly IR2Service _r2Service;

    public R2ServiceTests(IR2Service r2Service)
    {
        _r2Service = r2Service;
    }

    private static IFormFile CreateFormFile(string fileName, string contentType = "audio/mpeg", int size = 1024)
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.ContentType.Returns(contentType);
        file.Length.Returns(size);
        file.OpenReadStream().Returns(new MemoryStream(new byte[size]));
        return file;
    }

    // -------------------------
    // UploadSongStorage Tests
    // -------------------------

    [Fact]
    public async Task UploadSongStorage_Returns_FileName_When_Valid_Mp3()
    {
        var file = CreateFormFile("song.mp3");

        var result = await _r2Service.UploadSongStorage(file);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public async Task UploadSongStorage_Returns_FileName_When_Valid_Wav()
    {
        var file = CreateFormFile("song.wav", "audio/wav");

        var result = await _r2Service.UploadSongStorage(file);

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public async Task UploadSongStorage_Calls_Service_Once()
    {
        var file = CreateFormFile("song.mp3");

        await _r2Service.UploadSongStorage(file);

        await _r2Service.Received(1).UploadSongStorage(file);
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_File_Is_Null()
    {
        _r2Service.UploadSongStorage(null!).Returns<Task<string>>(_ => throw new ArgumentException("File is empty"));

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(null!));
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_File_Is_Empty()
    {
        var file = CreateFormFile("song.mp3", size: 0);
        _r2Service.UploadSongStorage(file).Returns<Task<string>>(_ => throw new ArgumentException("File is empty"));

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(file));
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_Invalid_Extension()
    {
        var file = CreateFormFile("song.exe", "application/octet-stream");
        _r2Service.UploadSongStorage(file).Returns<Task<string>>(_ => throw new ArgumentException("Invalid file type"));

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(file));
    }

    // -------------------------
    // GenerateSignedUrl Tests
    // -------------------------

    [Fact]
    public void GenerateSignedUrl_Returns_Url_For_Valid_Key()
    {
        var result = _r2Service.GenerateSignedUrl("some-file-key.mp3");

        Assert.False(string.IsNullOrWhiteSpace(result));
    }

    [Fact]
    public void GenerateSignedUrl_Returns_Different_Urls_For_Different_Keys()
    {
        _r2Service.GenerateSignedUrl("key1.mp3").Returns("https://signed-url.example.com/key1.mp3");
        _r2Service.GenerateSignedUrl("key2.mp3").Returns("https://signed-url.example.com/key2.mp3");

        var url1 = _r2Service.GenerateSignedUrl("key1.mp3");
        var url2 = _r2Service.GenerateSignedUrl("key2.mp3");

        Assert.NotEqual(url1, url2);
    }

    [Fact]
    public void GenerateSignedUrl_Called_With_Correct_Key()
    {
        var key = "my-song-" + Guid.NewGuid().ToString("N") + ".mp3";

        _r2Service.GenerateSignedUrl(key);

        _r2Service.Received(1).GenerateSignedUrl(key);
    }

    [Fact]
    public void GenerateSignedUrl_Respects_Custom_Expiration()
    {
        _r2Service.GenerateSignedUrl("key.mp3", 48).Returns("https://signed-url.example.com/key.mp3?exp=48");

        var result = _r2Service.GenerateSignedUrl("key.mp3", 48);

        _r2Service.Received(1).GenerateSignedUrl("key.mp3", 48);
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}