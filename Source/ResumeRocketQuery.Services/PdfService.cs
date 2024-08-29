using ResumeRocketQuery.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText;
using System.IO;
using System.Net.Http;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Layout.Element;
using iText.Layout;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using Org.BouncyCastle.Utilities;

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
                    FilteredEventListener listener = new FilteredEventListener();
                    LocationTextExtractionStrategy extractionStrategy = listener
                        .AttachEventListener(new LocationTextExtractionStrategy());
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(extractionStrategy);
                    parser.ProcessPageContent(document.GetFirstPage());
                    pageText.Append(extractionStrategy.GetResultantText());
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
                    FilteredEventListener listener = new FilteredEventListener();
                    LocationTextExtractionStrategy extractionStrategy = listener
                        .AttachEventListener(new LocationTextExtractionStrategy());
                    PdfCanvasProcessor parser = new PdfCanvasProcessor(extractionStrategy);
                    parser.ProcessPageContent(document.GetFirstPage());
                    pageText.Append(extractionStrategy.GetResultantText());
                }
                return pageText.ToString();
            }
        }

        public async Task<string> UpdatePdfAsync(string filepath, string update)
        {
            FileInfo file = new FileInfo(filepath);
            if (!file.Exists)
                file.Directory.Create();
            var writer = new PdfWriter(file);
            var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);
            doc.Add(new Paragraph(update));
            doc.Close();
            return file.FullName;
        }
    }
}
