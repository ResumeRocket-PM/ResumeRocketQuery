using System;
using System.Threading.Tasks;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResumeRocketQuery.Domain.External;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace ResumeRocketQuery.Repository.Tests
{
    public class PdfToHtmlClientTests
    {
        private IPdfToHtmlClient _systemUnderTest;

        public PdfToHtmlClientTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IPdfToHtmlClient>();
        }

        public class ConvertPdf : PdfToHtmlClientTests
        {
            [Fact]
            public async Task WHEN_Convert_is_called_THEN_response_is_NOT_NULL()
            {
                // Arrange
                using var memoryStream = new MemoryStream();

                var pdfBytes = File.ReadAllBytes("resume.pdf");

                await memoryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                memoryStream.Position = 0; 

                // Act
                var resultStream = await _systemUnderTest.ConvertPdf(memoryStream);

                var fileName = $"{Guid.NewGuid().ToString()}.html";

                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resultStream.Position = 0;
                    await resultStream.CopyToAsync(fileStream);
                }

                // Assert
                Assert.True(File.Exists(fileName), "The output HTML file was not created.");
            }

            [Fact]
            public async Task GIVEN_html_WHEN_Convert_is_called_THEN_response_is_NOT_NULL()
            {
                // Arrange
                using var memoryStream = new MemoryStream();
                var pdfBytes = File.ReadAllBytes("resume.pdf");

                await memoryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                memoryStream.Position = 0;

                // Act
                var htmlStream = await _systemUnderTest.ConvertPdf(memoryStream);

                var resultStream = await _systemUnderTest.StripHtmlElements(htmlStream);

                var fileName = $"{Guid.NewGuid().ToString()}.html";

                using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    resultStream.Position = 0; 
                    await resultStream.CopyToAsync(fileStream);
                }

                // Assert
                Assert.True(File.Exists(fileName), "The output HTML file was not created.");
            }
        }
    }
}
