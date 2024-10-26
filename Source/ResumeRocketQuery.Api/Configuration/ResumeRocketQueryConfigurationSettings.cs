using System.IO;
using ResumeRocketQuery.Domain.Configuration;
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Api.Configuration
{
    public class ResumeRocketQueryConfigurationSettings : IResumeRocketQueryConfigurationSettings
    {
        private readonly IConfigurationRoot _configurationRoot;

        public ResumeRocketQueryConfigurationSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configurationRoot = builder.Build();
        }

        public string Pdf2HtmlUrl => _configurationRoot.GetSection("AppSettings")["Pdf2HtmlUrl"];
        public string AuthenticationPrivateKey => _configurationRoot.GetSection("AppSettings")["AuthenticationPrivateKey"];
        public string ResumeRocketQueryDatabaseConnectionString => _configurationRoot.GetSection("ConnectionStrings")["ResumeRocketQueryDatabaseConnectionString"];
        public string BlobStorageConnectionString => _configurationRoot.GetSection("AzureBlobStorage")["ConnectionString"];
        public string BlobStorageContainerName => _configurationRoot.GetSection("AzureBlobStorage")["ContainerName"];
    }
}
