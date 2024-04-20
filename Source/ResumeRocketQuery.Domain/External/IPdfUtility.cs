using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.External
{
    public interface IPdfServices
    {
        Task<string> ReadPdfAsync(string filepath);
        Task<string> UpdatePdfAsync(string filepath);
    }
}
