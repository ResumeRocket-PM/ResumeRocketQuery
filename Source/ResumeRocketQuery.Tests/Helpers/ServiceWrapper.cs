using System;
using Microsoft.AspNetCore.Hosting;

namespace ResumeRocketQuery.Tests.Helpers
{
    public class ServiceWrapper : IDisposable
    {
        private readonly IDisposable _service;

        public readonly IServiceProvider ServiceProvider;
        public readonly string Url;

        public ServiceWrapper(IWebHost service, string url)
        {
            _service = service;
            ServiceProvider = service.Services;
            Url = url;
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}