using System.Text.Json;
using Amazon;
using Amazon.S3;
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

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var allowedExtensions = new[] { ".mp3", ".wav" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type");

        Console.WriteLine($"R2 Endpoint loaded: '{_endpoint}'");
        if (string.IsNullOrWhiteSpace(_endpoint))
        {
            throw new InvalidOperationException("R2 Endpoint is empty or null - check your JSON config file");
        }
        if (!_endpoint.StartsWith("https://") || !_endpoint.EndsWith(".r2.cloudflarestorage.com"))
        {
            Console.WriteLine("Warning: Endpoint does not look like a valid R2 URL");
        }

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
}
