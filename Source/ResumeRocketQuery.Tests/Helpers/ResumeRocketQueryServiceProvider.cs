using System;
using ResumeRocketQuery.Api.Configuration;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ResumeRocketQuery.Tests.Helpers
{
    public class ResumeRocketQueryServiceProvider
    {
        public IServiceProvider Create()
        {
            var container = new ServiceCollection();
            (new ResumeRocketQueryServiceCollection()).ConfigureServices(container);

            RegisterAll(container);

            var serviceProvider = container.BuildServiceProvider();

            return serviceProvider;
        }

        private void RegisterAll(IServiceCollection serviceCollection)
        {
            RegisterConfiguration(serviceCollection);
            RegisterMemoryFake(serviceCollection);
        }

        private void RegisterConfiguration(IServiceCollection serviceCollection)
        {
            var mock = new Mock<IResumeRocketQueryConfigurationSettings>();
            mock.Setup(x => x.AuthenticationPrivateKey).Returns("testConfigurationSettings");
            mock.Setup(x => x.ResumeRocketQueryDatabaseConnectionString).Returns("Data Source=localhost,1433;Initial Catalog=ResumeRocketQueryService;Application Name=ResumeRocketQueryApi;User Id=SA;Password=!QAZ2wsx;");

            serviceCollection.AddSingleton(mock.Object);
        }

        private void RegisterMemoryFake(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IResumeRocketQueryStorage, MemoryResumeRocketQueryStorage>();
        }
    }
}
