using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;

public class AzureService : IAzureService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;

    public AzureService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration["BlobConnectionString"]);
        _blobContainerName = configuration["BlobContainerName"] ?? throw new ArgumentNullException(nameof(_blobContainerName), "BlobContainerName is required in configuration");
    }

    public async Task<UploadResultDto> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty", nameof(file));
        }

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return new UploadResultDto
            {
                FileName = uniqueFileName,
                Url = blobClient.Uri.ToString(),
                UploadDate = DateTime.UtcNow,
                FileSize = file.Length
            };
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string blobFilename)
    {
        if (string.IsNullOrWhiteSpace(blobFilename))
        {
            throw new ArgumentException("Blob filename cannot be null or empty", nameof(blobFilename));
        }

        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = containerClient.GetBlobClient(blobFilename);
            var response = await blobClient.DeleteIfExistsAsync();

            return response.Value;
        }
        catch
        {
            throw;
        }
    }
}