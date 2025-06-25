using System;
using ResumeRocketQuery.Api.Configuration;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Extensions.Configuration;
using ResumeRocketQuery.Tests.ResourceBuilder;
using System.Collections.Generic;

namespace ResumeRocketQuery.Tests.Helpers
{
    public class ResumeRocketQueryServiceProvider
    {
        public IServiceProvider Create()
        {
            var container = new ServiceCollection();

            // Create a test-specific configuration
            var testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                { "ConnectionStrings:ResumeRocketQueryDatabaseConnectionString", "Server=localhost; Database=ResumeRocketTest; Trusted_Connection=True; TrustServerCertificate=True;" }
                })
                .Build();

            // Pass the test configuration to the service collection
            (new ResumeRocketQueryServiceCollection()).ConfigureServices(container, testConfiguration);

            RegisterAll(container);

            var serviceProvider = container.BuildServiceProvider();

            return serviceProvider;
        }

        private void RegisterAll(IServiceCollection serviceCollection)
        {
            RegisterMemoryFake(serviceCollection);

            serviceCollection.AddSingleton<TestResourceBuilder>();
        }

        private void RegisterMemoryFake(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddSingleton<IResumeRocketQueryStorage, MemoryResumeRocketQueryStorage>();
        }
    }
}
