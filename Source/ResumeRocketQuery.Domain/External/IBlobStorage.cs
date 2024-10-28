using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IBlobStorage
    {
        Task<(string, string)> UploadOrOverwriteImageAsync(IFormFile file, string imageId = null);

        string GenerateReadOnlySasToken();
    }
}
