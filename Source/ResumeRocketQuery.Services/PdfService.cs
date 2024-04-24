using ResumeRocketQuery.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using System.IO;
using System.Net.Http;

namespace ResumeRocketQuery.External
{
    public class PdfService : IPdfService
    {
        public async Task<string> ReadPdfAsync(string filepath)
        {
            StringBuilder resumeText = new StringBuilder();
            using (var pdf = PdfDocument.Open(filepath))
            {
                foreach (var page in pdf.GetPages())
                {
                    // Extract based on order in the underlying document with newlines and spaces.
                    var text = ContentOrderTextExtractor.GetText(page);
                    resumeText.Append(text);
                }
            }
            return resumeText.ToString();
        }

        public async Task<string> ReadPdfAsync(MemoryStream bytes)
        {
            StringBuilder resumeText = new StringBuilder();
            using (var pdf = PdfDocument.Open(bytes))
            {
                foreach (var page in pdf.GetPages())
                {
                    // Extract based on order in the underlying document with newlines and spaces.
                    var text = ContentOrderTextExtractor.GetText(page);
                    resumeText.Append(text);
                }
            }
            return resumeText.ToString();
        }

        public async Task<string> UpdatePdfAsync(string filepath, string update)
        {
            using (PdfDocumentBuilder builder = new PdfDocumentBuilder())
            {
                PdfPageBuilder page = builder.AddPage(PageSize.A4);
                // Fonts must be registered with the document builder prior to use to prevent duplication.
                PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
                page.AddText(update, 12, new PdfPoint(25, 700), font);
                byte[] documentBytes = builder.Build();
                File.WriteAllBytes(filepath, documentBytes);
            }
            return filepath;
        }
    }
}
