namespace ResumeRocketQuery.Domain.Configuration
{
    public interface IResumeRocketQueryConfigurationSettings
    {
        string AuthenticationPrivateKey { get; }
        string ResumeRocketQueryDatabaseConnectionString { get; }
    }
}
