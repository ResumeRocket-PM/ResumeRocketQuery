namespace ResumeRocketQuery.Domain.Configuration
{
    public interface IResumeRocketQueryConfigurationSettings
    {
        string AuthenticationPrivateKey { get; }
        string ResumeRocketQueryDatabaseConnectionString { get; }
        string BlobStorageConnectionString { get; }
        string BlobStorageContainerName { get; }
        string Pdf2HtmlUrl { get; }
        string LlamaClientUrl { get; }
        string AuthenticationIssuer { get; }
        string AuthenticationAudience { get; }
    }
}
