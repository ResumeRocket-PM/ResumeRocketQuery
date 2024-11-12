using Aspose.Words.Loading;
using Aspose.Words;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Document = Aspose.Words.Document;

namespace ResumeRocketQuery.Services
{
    public interface IFileService
    {
        Task<string> ConvertHtml(Stream fileStream);
    }

    public class FileService : IFileService
    {
        public async Task<string> ConvertHtml(Stream fileStream)
        {
            // Load the HTML content from the input stream
            var loadOptions = new HtmlLoadOptions { Encoding = System.Text.Encoding.UTF8 };

            Document doc = new Document(fileStream, loadOptions);

            MemoryStream wordStream = new MemoryStream();

            doc.Save("output.docx", SaveFormat.Docx);

            byte[] byteArray = wordStream.ToArray();

            string base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }


    }
}
