using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using System.Threading.Tasks;
using System;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Api.Controllers
{
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IImageService _imageService;

        public ImageController(IServiceResponseBuilder serviceResponseBuilder, IImageService imageService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _imageService = imageService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            string imageUrl = await _imageService.UploadImageAsync(file);
            return Ok(new { ImageUrl = imageUrl });
        }

        [HttpGet("generateSasToken")]
        public IActionResult GenerateSasToken()
        {
            string sasToken = _imageService.GenerateReadOnlySasToken();
            return Ok(new { SasToken = sasToken });
        }
    }
}
