using Microsoft.AspNetCore.Http;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class ImageService : IImageService
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IImageDataLayer _imageDataLayer;

        public ImageService(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            // You can add business logic here, like validating the file, checking file size, etc.
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Invalid image file.");
            }

            // Call the BlobStorage class to upload the image and get the URL
            string imageUrl = await _blobStorage.UploadImageAsync(image);

            // idk what we want to do with imageUrl or imageId yet. portfolio doesn't need either. 
            //int imageId = await _imageDataLayer.InsertImageRecordAsync(imageUrl, image.FileName);

            return imageUrl;
        }

        public string GenerateReadOnlySasToken()
        {
            return _blobStorage.GenerateReadOnlySasToken();
        }
    }
}
