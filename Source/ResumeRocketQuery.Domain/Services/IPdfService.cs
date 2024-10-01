using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IPdfService
    {
        Task<string> ReadPdfAsync(string filepath);
        Task<string> ReadPdfAsync(MemoryStream bytes);
        Task<string> UpdatePdfAsync(string filepath, string update);

        Task<string> CreatePdfAsync(string filename, string html);
    }
}
