using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.External
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;


        public BlobStorage(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _blobServiceClient = new BlobServiceClient(resumeRocketQueryConfigurationSettings.BlobStorageConnectionString);
            _containerName = resumeRocketQueryConfigurationSettings.BlobStorageContainerName;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            var blobClient = containerClient.GetBlobClient(file.FileName);
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Log or throw a specific error message
                Console.WriteLine($"Error uploading file to Blob Storage: {ex.Message}");
                throw new Exception("File upload to Blob Storage failed.", ex);
            }

            // Return the URL of the uploaded image
            return blobClient.Uri.ToString();
        }

        public string GenerateReadOnlySasToken()
        {
            // Get a reference to the container
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            // Define the expiration time for the SAS token
            var expirationTime = DateTimeOffset.UtcNow.AddHours(1); // Set to 1 hour from now

            // Create a SAS token with read-only permissions
            var permissions = BlobContainerSasPermissions.Read; // Set read permissions


            // Generate the SAS token with the specified permissions and expiration
            var sasToken = containerClient.GenerateSasUri(permissions, expirationTime);

            // Return the SAS token as a string
            return sasToken.ToString();
        }
    }
}
