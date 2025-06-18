using System.IO;
using ResumeRocketQuery.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using System;

namespace ResumeRocketQuery.Api.Configuration
{
    public class ResumeRocketQueryConfigurationSettings : IResumeRocketQueryConfigurationSettings
    {
        private readonly IConfigurationRoot _configurationRoot;

        public ResumeRocketQueryConfigurationSettings(IConfiguration configuration = null)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            if (configuration != null)
            {
                builder.AddConfiguration(configuration); // Add custom configuration for tests
            }

            _configurationRoot = builder.Build();
        }

        public string Pdf2HtmlUrl => _configurationRoot.GetSection("AppSettings")["Pdf2HtmlUrl"];
        public string LlamaClientUrl => _configurationRoot.GetSection("AppSettings")["LlamaClientUrl"];
        public string AuthenticationPrivateKey => _configurationRoot.GetSection("AppSettings")["AuthenticationPrivateKey"];
        public string AuthenticationIssuer => _configurationRoot.GetSection("AppSettings")["AuthenticationIssuer"];
        public string AuthenticationAudience => _configurationRoot.GetSection("AppSettings")["AuthenticationAudience"];
        public string ResumeRocketQueryDatabaseConnectionString => _configurationRoot.GetSection("ConnectionStrings")["ResumeRocketQueryDatabaseConnectionString"];
        public string BlobStorageConnectionString => _configurationRoot.GetSection("AzureBlobStorage")["ConnectionString"];
        public string BlobStorageContainerName => _configurationRoot.GetSection("AzureBlobStorage")["ContainerName"];
        public string OpenAI_API_Key => _configurationRoot.GetSection("OpenAi")["API_key"];
    }
}
