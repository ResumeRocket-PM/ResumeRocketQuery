using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Api.Tests.Helpers;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Tests.Helpers;
using Xunit;
using ResumeRocketQuery.Domain.Api;
using Microsoft.Identity.Client;
using ResumeRocketQuery.Domain.DataLayer;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ResumeRocketQuery.Api.Tests
{
    public class ImageControllerTests
    {
        private readonly RestRequestClient _restRequestClient;

        public ImageControllerTests()
        {
            _restRequestClient = new RestRequestClient();
        }

        public class Get : ImageControllerTests
        {
            [Fact]
            public async Task WHEN_UPLOAD_is_called_THEN_image_URL_not_empty()
            {
                using (var selfHost = new WebApiStarter().Start(typeof(Startup)))
                {
                    var imageService = selfHost.ServiceProvider.GetService<IImageService>();

                    var image = new FormFile(new MemoryStream(new byte[] { 0x20, 0x20, 0x20 }), 0, 3, "Data", "Data.txt");

                    var result = await imageService.UploadImageAsync(image);

                    Assert.NotNull(result);

                }
            }

            [Fact]
            public async Task WHEN_GENERATE_SAS_TOKEN_is_called_THEN_sas_token_not_empty()
            {
                using (var selfHost = new WebApiStarter().Start(typeof(Startup)))
                {
                    var imageService = selfHost.ServiceProvider.GetService<IImageService>();

                    var result = imageService.GenerateReadOnlySasToken();

                    Assert.NotNull(result);

                }
            }
        }

    }
}
