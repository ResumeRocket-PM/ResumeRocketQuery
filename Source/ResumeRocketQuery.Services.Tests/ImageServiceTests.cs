using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Api;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using ResumeRocketQuery.Tests.ResourceBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class ImageServiceTests
    {
        private readonly IImageService _imageService;

        public ImageServiceTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _imageService = serviceProvider.GetService<IImageService>();
        }

        public class Get : ImageServiceTests
        {
            [Fact]
            public async Task WHEN_UPLOAD_is_called_AND_empty_image_id_THEN_result_not_empty()
            {
                var image = new FormFile(new MemoryStream(new byte[] { 0x20, 0x20, 0x20 }), 0, 3, "Data", "Data.txt");
                var imageId = "";

                var result = await _imageService.UploadImageAsync(image, imageId);

                Assert.NotNull(result.Item1);
                Assert.NotNull(result.Item2);
            }

            [Fact]
            public async Task WHEN_UPLOAD_is_called_AND_existing_image_id_THEN_overwrite_existing_image()
            {
                // upload an image, then upload another image with the same imageId
                var image = new FormFile(new MemoryStream(new byte[] { 0x20, 0x20, 0x20 }), 0, 3, "Data", "Data.txt");
                var imageId = "test";

                var result = await _imageService.UploadImageAsync(image, imageId);
                Assert.NotNull(result.Item1);
                Assert.NotNull(result.Item2);

                var result2 = await _imageService.UploadImageAsync(image, imageId);
                Assert.NotNull(result2.Item1);
                Assert.NotNull(result2.Item2);
                Assert.Equal(result.Item1, result2.Item1);
            }

            [Fact]
            public async Task WHEN_GENERATE_SAS_TOKEN_is_called_THEN_sas_token_not_empty()
            {
                var result = _imageService.GenerateReadOnlySasToken();

                Assert.NotNull(result);
            }

        }

    }
}
