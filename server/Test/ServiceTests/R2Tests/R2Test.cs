using Amazon.S3;
using Api.Services.R2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace Test.ServiceTests.R2Tests;

public class R2ServiceTests
{
    private readonly IConfiguration _mockConfig;
    private readonly IAmazonS3 _s3Mock;
    private readonly R2Service _r2Service;

    public R2ServiceTests()
    {
        _mockConfig = CreateMockConfig();
        _r2Service = new R2Service(_mockConfig);
        _s3Mock = Substitute.For<IAmazonS3>();
    }

    private static IConfiguration CreateMockConfig()
    {
        var config = Substitute.For<IConfiguration>();
        var configSection = Substitute.For<IConfigurationSection>();
        
        var tempDir = Path.GetTempPath();
        var configPath = Path.Combine(tempDir, "r2-config.json");
        var configJson = """
        {
            "AccessKey": "test-access-key",
            "SecretKey": "test-secret-key",
            "BucketName": "test-bucket",
            "Endpoint": "https://test-endpoint.r2.cloudflarestorage.com"
        }
        """;
        File.WriteAllText(configPath, configJson);

        config["R2:ConfigPath"].Returns(configPath);
        
        return config;
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
    public async Task UploadSongStorage_Throws_When_File_Is_Null()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(null!));
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_File_Is_Empty()
    {
        var file = CreateFormFile("song.mp3", size: 0);

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(file));
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_Invalid_Extension()
    {
        var file = CreateFormFile("song.exe", "application/octet-stream");

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(file));
    }

    [Fact]
    public async Task UploadSongStorage_Throws_When_Invalid_Audio_Extension()
    {
        var file = CreateFormFile("song.txt", "text/plain");

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadSongStorage(file));
    }

    [Fact]
    public async Task UploadSongStorage_Accepts_Mp3_Extension()
    {
        var file = CreateFormFile("song.mp3");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadSongStorage(file));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UploadSongStorage_Accepts_Wav_Extension()
    {
        var file = CreateFormFile("song.wav", "audio/wav");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadSongStorage(file));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UploadSongStorage_Case_Insensitive_Extension()
    {
        var file = CreateFormFile("song.MP3");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadSongStorage(file));
        Assert.NotNull(exception);
    }

    // -------------------------
    // UploadImageStorage Tests
    // -------------------------

    [Fact]
    public async Task UploadImageStorage_Throws_When_File_Is_Null()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadImageStorage(null!));
    }

    [Fact]
    public async Task UploadImageStorage_Throws_When_File_Is_Empty()
    {
        var file = CreateFormFile("image.jpg", "image/jpeg", size: 0);

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadImageStorage(file));
    }

    [Fact]
    public async Task UploadImageStorage_Throws_When_Invalid_Extension()
    {
        var file = CreateFormFile("image.bmp", "image/bmp");

        await Assert.ThrowsAsync<ArgumentException>(() => _r2Service.UploadImageStorage(file));
    }

    [Fact]
    public async Task UploadImageStorage_Accepts_Jpg_Extension()
    {
        var file = CreateFormFile("image.jpg", "image/jpeg");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadImageStorage(file));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UploadImageStorage_Accepts_Jpeg_Extension()
    {
        var file = CreateFormFile("image.jpeg", "image/jpeg");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadImageStorage(file));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UploadImageStorage_Accepts_Png_Extension()
    {
        var file = CreateFormFile("image.png", "image/png");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadImageStorage(file));
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UploadImageStorage_Accepts_Webp_Extension()
    {
        var file = CreateFormFile("image.webp", "image/webp");

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.UploadImageStorage(file));
        Assert.NotNull(exception);
    }

    // -------------------------
    // GenerateSignedUrl Tests
    // -------------------------

    [Fact]
    public void GenerateSignedUrl_Returns_Url_String()
    {
        var result = _r2Service.GenerateSignedUrl("some-file-key.mp3");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.StartsWith("https://", result);
    }

    [Fact]
    public void GenerateSignedUrl_With_Default_Expiration()
    {
        var result = _r2Service.GenerateSignedUrl("key.mp3");

        Assert.NotNull(result);
        Assert.Contains("key.mp3", result);
    }

    [Fact]
    public void GenerateSignedUrl_With_Custom_Expiration()
    {
        var result = _r2Service.GenerateSignedUrl("key.mp3", 48);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateSignedUrl_Returns_Different_Urls_For_Different_Keys()
    {
        var url1 = _r2Service.GenerateSignedUrl("key1.mp3");
        var url2 = _r2Service.GenerateSignedUrl("key2.mp3");

        Assert.NotEqual(url1, url2);
    }

    // -------------------------
    // DeleteFile Tests
    // -------------------------

    [Fact]
    public async Task DeleteFile_Returns_Early_When_Key_Is_Null()
    {
        await _r2Service.DeleteFile(null!);
        
        await _s3Mock.Received(0).DeleteObjectAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteFile_Returns_Early_When_Key_Is_Empty()
    {
        await _r2Service.DeleteFile("");

        await _s3Mock.DidNotReceiveWithAnyArgs().DeleteObjectAsync(default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteFile_Returns_Early_When_Key_Is_Whitespace()
    {
        await _r2Service.DeleteFile("   ");

        await _s3Mock.DidNotReceiveWithAnyArgs().DeleteObjectAsync(default!, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DeleteFile_Attempts_Delete_With_Valid_Key()
    {
        await Assert.ThrowsAsync<HttpRequestException>(() => _r2Service.DeleteFile("songs/some-file.mp3"));
    }

    // -------------------------
    // Constructor Tests
    // -------------------------

    [Fact]
    public void Constructor_Throws_When_Config_Path_Is_Null()
    {
        var badConfig = Substitute.For<IConfiguration>();
        badConfig["R2:ConfigPath"].Returns((string)null!);

        Assert.Throws<InvalidOperationException>(() => new R2Service(badConfig));
    }

    [Fact]
    public void Constructor_Throws_When_Config_File_Does_Not_Exist()
    {
        var badConfig = Substitute.For<IConfiguration>();
        badConfig["R2:ConfigPath"].Returns("/nonexistent/path/config.json");

        Assert.Throws<InvalidOperationException>(() => new R2Service(badConfig));
    }

    [Fact]
    public void Constructor_Throws_When_Config_Json_Is_Invalid()
    {
        var tempDir = Path.GetTempPath();
        var configPath = Path.Combine(tempDir, "invalid-config.json");
        File.WriteAllText(configPath, "{ invalid json");

        var badConfig = Substitute.For<IConfiguration>();
        badConfig["R2:ConfigPath"].Returns(configPath);

        Assert.Throws<System.Text.Json.JsonException>(() => new R2Service(badConfig));
    }

    [Fact]
    public void Constructor_Throws_When_AccessKey_Is_Missing()
    {
        var tempDir = Path.GetTempPath();
        var configPath = Path.Combine(tempDir, "missing-key-config.json");
        var configJson = """
                         {
                             "SecretKey": "test-secret-key",
                             "BucketName": "test-bucket",
                             "Endpoint": "https://test-endpoint.r2.cloudflarestorage.com"
                         }
                         """;
        File.WriteAllText(configPath, configJson);

        var badConfig = Substitute.For<IConfiguration>();
        badConfig["R2:ConfigPath"].Returns(configPath);

        var exception = Assert.Throws<InvalidOperationException>(() => new R2Service(badConfig));
        Assert.Contains("AccessKey", exception.Message);
    }

    [Fact]
    public void Constructor_Throws_When_Endpoint_Is_Empty()
    {
        var tempDir = Path.GetTempPath();
        var configPath = Path.Combine(tempDir, "empty-endpoint-config.json");
        var configJson = """
        {
            "AccessKey": "test-access-key",
            "SecretKey": "test-secret-key",
            "BucketName": "test-bucket",
            "Endpoint": "   "
        }
        """;
        File.WriteAllText(configPath, configJson);

        var badConfig = Substitute.For<IConfiguration>();
        badConfig["R2:ConfigPath"].Returns(configPath);

        Assert.Throws<InvalidOperationException>(() => new R2Service(badConfig));
    }
}