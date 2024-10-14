using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IPdfToHtmlClient _PdfToHtmlClient;
        private readonly IResumeDataLayer _resumeDataLayer;

        public ResumeService(IPdfToHtmlClient pdfToHtmlClient, IResumeDataLayer resumeDataLayer)
        {
            _PdfToHtmlClient = pdfToHtmlClient;
            _resumeDataLayer = resumeDataLayer;
        }

        public async Task<ResumeResult> CreateResumeFromPdf(ResumeRequest request)
        {
            var pdfBytes = Convert.FromBase64String(request.Pdf["FileBytes"]);

            var pdfStream = new MemoryStream(pdfBytes);

            var htmlStream = await _PdfToHtmlClient.ConvertPdf(pdfStream);

            using (StreamReader reader = new StreamReader(htmlStream))
            {
                var html = reader.ReadToEnd();

                var resumeId = await _resumeDataLayer.InsertResumeAsync(new ResumeStorage
                {
                    AccountId = request.AccountId,
                    Resume = html,
                });

                var result = new ResumeResult
                {
                    Html = html,
                    ResumeId = resumeId,
                };

                return result;
            }
        }
    }
}
