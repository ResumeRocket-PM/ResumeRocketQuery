using System;
using ResumeRocketQuery.Api.Configuration;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.Extensions.Configuration;

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
            RegisterMemoryFake(serviceCollection);
        }

        private void RegisterMemoryFake(IServiceCollection serviceCollection)
        {
            //serviceCollection.AddSingleton<IResumeRocketQueryStorage, MemoryResumeRocketQueryStorage>();
        }
    }
}
