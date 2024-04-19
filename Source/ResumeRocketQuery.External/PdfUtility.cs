using ResumeRocketQuery.Domain.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.External
{
    public class PdfUtility : IPdfUtility
    {
        public Task<string> ReadPdfAsync(string filepath)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdatePdfAsync(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
