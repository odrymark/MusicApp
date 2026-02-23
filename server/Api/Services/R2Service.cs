using System.Text.Json;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

public class R2Service
{
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _bucketName;
    private readonly string _endpoint;

    public R2Service(IConfiguration config)
    {
        var path = config["R2:ConfigPath"];
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            throw new InvalidOperationException($"R2 config file not found or path empty: '{path}'");
        }

        var json = File.ReadAllText(path);

        var r2Config = JsonSerializer.Deserialize<R2Config>(json);

        if (r2Config == null)
        {
            throw new InvalidOperationException("Failed to deserialize R2 config JSON");
        }

        _accessKey = r2Config.AccessKey ?? throw new ArgumentNullException(nameof(r2Config.AccessKey));
        _secretKey = r2Config.SecretKey ?? throw new ArgumentNullException(nameof(r2Config.SecretKey));
        _bucketName = r2Config.BucketName ?? throw new ArgumentNullException(nameof(r2Config.BucketName));
        _endpoint = r2Config.Endpoint ?? throw new ArgumentNullException(nameof(r2Config.Endpoint));

        if (string.IsNullOrWhiteSpace(_endpoint))
        {
            throw new InvalidOperationException("R2 Endpoint is empty or whitespace in config file");
        }
    }

    private class R2Config
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Endpoint { get; set; }
    }

    public async Task<string> UploadSongStorage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var allowedExtensions = new[] { ".mp3", ".wav" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type");

        var s3Config = new AmazonS3Config
        {
            ServiceURL = _endpoint,
            ForcePathStyle = true,
            AuthenticationRegion = "auto"
        };

        using var s3Client = new AmazonS3Client(_accessKey, _secretKey, s3Config);
        var fileTransferUtility = new TransferUtility(s3Client);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";

        await using var stream = file.OpenReadStream();

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            BucketName = _bucketName,
            Key = fileName,
            ContentType = file.ContentType ?? "application/octet-stream",
            DisablePayloadSigning = true,
        };

        await fileTransferUtility.UploadAsync(uploadRequest);

        return fileName;
    }
    
    public string GenerateSignedUrl(string key, int expirationHours = 1)
    {
        var s3Config = new AmazonS3Config
        {
            ServiceURL = _endpoint,
            ForcePathStyle = true,
            AuthenticationRegion = "auto"
        };

        using var s3Client = new AmazonS3Client(_accessKey, _secretKey, s3Config);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddHours(expirationHours)
        };

        return s3Client.GetPreSignedURL(request);
    } 
}
