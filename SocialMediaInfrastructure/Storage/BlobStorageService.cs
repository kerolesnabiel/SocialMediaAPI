using Azure.Storage.Blobs;
using SocialMediaDomain.Interfaces;

namespace SocialMediaInfrastructure.Storage;

internal class BlobStorageService(BlobServiceClient blobServiceClient) : IBlobStorageService
{
    public async Task<string> UploadToBlobAsync(Stream data, string filename, string containerName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        var blobClient = containerClient.GetBlobClient(filename);

        await blobClient.UploadAsync(data);

        return blobClient.Uri.ToString();
    }
}
