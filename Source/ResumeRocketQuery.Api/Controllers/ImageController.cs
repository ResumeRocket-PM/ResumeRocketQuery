using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using System.Threading.Tasks;
using System;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Api.Builder;
using System.IO;
using Microsoft.AspNetCore.Http.HttpResults;


//******* IMPORTANT: 
//    images must have unique filenames. I've set UploadOrOverwriteImageAsync to overwrite the image in the 
//    blob storage if the filename already exists. t
//    the imageId becomes the filename of any image uploaded 



namespace ResumeRocketQuery.Api.Controllers
{
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IImageService _imageService;
        private readonly IPortfolioService _portfolioService;

        public ImageController(
            IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IImageService imageService, 
            IPortfolioService portfolioService
        )
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _imageService = imageService;
            _portfolioService = portfolioService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, string imageId)
        {
            (string _imageUrl, string _imageId) = await _imageService.UploadImageAsync(file, imageId);
            return Ok(new { ImageUrl = _imageUrl, ImageId = _imageId});
        }

        [HttpGet("generateSasToken")]
        public IActionResult GenerateSasToken()
        {
            string sasToken = _imageService.GenerateReadOnlySasToken();
            return Ok(new { SasToken = sasToken });
        }
    }
}
