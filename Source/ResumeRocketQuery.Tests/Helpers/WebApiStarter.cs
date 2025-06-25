using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace ResumeRocketQuery.Tests.Helpers
{
    public class WebApiStarter
    {
        private object _lockObject = new object();
        private readonly IConfiguration _configuration;

        public WebApiStarter(IConfiguration configuration = null)
        {
            _configuration = configuration;
        }

        public ServiceWrapper Start(Type webApiStartup)
        {
            string url = $"http://localhost:{GetFreeTcpPort()}";

            var service = WebHost.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (_configuration != null)
                    {
                        config.AddConfiguration(_configuration);
                    }
                })
                .UseStartup(webApiStartup)
                .UseUrls(url)
                .Build();

            service.Start();

            return new ServiceWrapper(service, url);
        }

        private int GetFreeTcpPort()
        {
            lock (_lockObject)
            {
                TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);

                tcpListener.Start();

                int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;

                tcpListener.Stop();

                return port;
            }
        }
    }
}