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
            mock.Setup(x => x.AuthenticationPrivateKey).Returns("X3hc+qbzsu5ANqy5nLiCM+jTZGzhfpjWa60Bbrjfz0dIAgkmQRcu72fJoAXZis93u54lbVvV2xDoY3naNyk+1R++d5xNCPFg1sFaHm9M8QnQF5DacZvrHrP6bFx+ifiVhcbTxuKwoZieUxc6QFk6VO+CI6B8FhWOkdcjplmPws8SGADQAcyAbYqPqWEVfOoPAwgsZVNUlQSPR4yw4hUxRcn9pZRm8nWNyVogKLk7YgkQBHnk8VryAK+8qp6fmjc9CZwb7Oc4lXSlHeLi2N/H896qkUhMo5XfKhGApRq9C6l8+UsWENMfzmYrv4M/COcG+biicJUj1WLGwGIzEhY7En+8G81tkvGz9dpX8+IHaIE=");
            mock.Setup(x => x.ResumeRocketQueryDatabaseConnectionString).Returns("Server=tdebruin.net; Port=3306; Database=ResumeRocket; User ID=resumerocket; Password=852421655;");

            serviceCollection.AddSingleton(mock.Object);
        }

        private void RegisterMemoryFake(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IResumeRocketQueryStorage, MemoryResumeRocketQueryStorage>();
        }
    }
}
