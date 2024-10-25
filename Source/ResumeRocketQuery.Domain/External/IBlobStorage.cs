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
        Task<string> UploadImageAsync(IFormFile file);

        string GenerateReadOnlySasToken();
    }
}
