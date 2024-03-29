using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ResumeRocketQuery.Tests.Helpers
{
    public class WebApiStarter
    {
        private object _lockObject = new object();

        public ServiceWrapper Start(Type webApiStartup)
        {
            string url = $"http://localhost:{GetFreeTcpPort()}";

            var service = WebHost.CreateDefaultBuilder()
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