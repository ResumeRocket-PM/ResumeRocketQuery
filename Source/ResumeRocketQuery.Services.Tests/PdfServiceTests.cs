using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResumeRocketQuery.Services.Tests
{
    public class PdfServiceTests
    {
        private IPdfService _systemUnderTest;

        public PdfServiceTests() 
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _systemUnderTest = serviceProvider.GetService<IPdfService>();
        }

        public class ReadPdfAsync : PdfServiceTests
        {
            [Fact]
            public async Task WHEN_ReadPdfAsync_is_called_THEN_response_is_NOT_NULL()
            {
                var response = await _systemUnderTest.ReadPdfAsync(@"./Samples/studentAthleteResume.pdf");
                Assert.True(response != null);
            }
        }

        public class UpdatePdfAsync : PdfServiceTests
        {
            [Fact]
            public async Task WHEN_UpdatePdfAsync_is_called_THEN_response_is_OK()
            {
                var response = await _systemUnderTest.UpdatePdfAsync(@"./Samples/studentAthleteResume.pdf", "Here is the resume update");
                Assert.True(response != null);
            }
        }
    }
}
