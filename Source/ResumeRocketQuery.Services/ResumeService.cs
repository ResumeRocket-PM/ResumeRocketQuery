using iText.Html2pdf;
using iText.Kernel.Pdf;
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
        private readonly IAccountService _accountService;

        public ResumeService(IPdfToHtmlClient pdfToHtmlClient, IResumeDataLayer resumeDataLayer, IAccountService accountService)
        {
            _PdfToHtmlClient = pdfToHtmlClient;
            _resumeDataLayer = resumeDataLayer;
            _accountService = accountService;
        }

        public async Task<string> GetPrimaryResume(int accountId)
        {
            var user = await _accountService.GetAccountAsync(accountId);

            string result = null;

            if (user.PrimaryResumeId.HasValue)
            {
                var resume = await _resumeDataLayer.GetResumeAsync(user.PrimaryResumeId.Value);

                result = resume.Resume;
            }

            return result;
        }

        public async Task<byte[]> GetPrimaryResumePdf(int accountId)
        {
            var html = await GetPrimaryResume(accountId);

            return ConvertFromHtml(html);
        }

        public async Task CreatePrimaryResume(ResumeRequest request)
        {
            var result = await CreateResumeFromPdf(request);

            await _accountService.UpdateAccount(request.AccountId, 
                new System.Collections.Generic.Dictionary<string, string>
                {
                    { "PrimaryResumeId", result.ResumeId.ToString() }
                });
        }

        public async Task<string> GetResume(int resumeId)
        {
            var resume = await _resumeDataLayer.GetResumeAsync(resumeId);

            string result = resume.Resume;

            return result;
        }

        public async Task<byte[]> GetResumePdf(int resumeId)
        {
            var resume = await _resumeDataLayer.GetResumeAsync(resumeId);

            return ConvertFromHtml(resume.Resume);
        }

        private byte[] ConvertFromHtml(string html)
        {
            if(html == null)
            {
                return new byte[0];
            }

            using (MemoryStream pdfStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(pdfStream);

                ConverterProperties converterProperties = new ConverterProperties();

                HtmlConverter.ConvertToPdf(html, writer, converterProperties);

                return pdfStream.ToArray();
            }
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
