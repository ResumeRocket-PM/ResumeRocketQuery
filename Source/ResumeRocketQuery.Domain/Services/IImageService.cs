using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IImageService
    {
        Task<(string, string)> UploadImageAsync(IFormFile file, string imageId);
        string GenerateReadOnlySasToken();
    }
}
