using ResumeRocketQuery.Domain.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iText.Kernel.Pdf;
using iText.Html2pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout.Element;
using iText.Layout;

namespace ResumeRocketQuery.Service
{
    public class PdfService : IPdfService
    {
        public async Task<string> ReadPdfAsync(string filepath)
        {
            var pageText = new StringBuilder();
            using (PdfDocument document = new PdfDocument(new PdfReader(filepath)))
            {
                var pageNumbers = document.GetNumberOfPages();
                for (int page = 1; page <= pageNumbers; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    pageText.Append(PdfTextExtractor.GetTextFromPage(document.GetPage(page), strategy));
                }
                return pageText.ToString();
            }
        }

        public async Task<string> ReadPdfAsync(MemoryStream bytes)
        {
            var pageText = new StringBuilder();
            using (PdfDocument document = new PdfDocument(new PdfReader(bytes)))
            {
                var pageNumbers = document.GetNumberOfPages();
                for (int page = 1; page <= pageNumbers; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    pageText.Append(PdfTextExtractor.GetTextFromPage(document.GetPage(page), strategy));
                }
                return pageText.ToString();
            }
        }

        public async Task<string> UpdatePdfAsync(string filepath, string update)
        {
            string name = filepath.Replace(".pdf", "");
            FileInfo file = new FileInfo(name+"-"+DateTime.Now.ToString("yyyyMMddHHmmffff")+".pdf");
            if (!file.Exists)
                file.Directory.Create();
            var writer = new PdfWriter(file);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            doc.Add(new Paragraph(update));
            doc.Close();
            return file.FullName;
        }

        public async Task<string> CreatePdfAsync(string filepath, string html)
        {
            string name = filepath.Replace(".pdf", "");
            FileInfo file = new FileInfo(name + "-" + DateTime.Now.ToString("yyyyMMddHHmmffff") + ".pdf");
            if (!file.Exists)
                file.Directory.Create();
            var writer = new PdfWriter(file);
            ConverterProperties converterProperties = new ConverterProperties();
            converterProperties.SetBaseUri(filepath);
            HtmlConverter.ConvertToPdf(html, writer, converterProperties);            
            return file.FullName;
        }
    }
}
