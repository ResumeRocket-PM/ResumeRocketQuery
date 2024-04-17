using Microsoft.Extensions.DependencyInjection;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ResumeRocketQuery.External.Tests
{
    public class PdfUtilityTests
    {
        private IPdfUtility _systemUnderTest;

        public PdfUtilityTests() 
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();
            _systemUnderTest = serviceProvider.GetService<IPdfUtility>();
        }

        public class ReadPdfAsync : PdfUtilityTests
        {
            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_THEN_response_is_NOT_NULL()
            {
                var response = await _systemUnderTest.ReadPdfAsync(@"path/to/file.pdf");

                Assert.True(response != null);
            }
        }

        public class UpdatePdfAsync : PdfUtilityTests
        {
            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_THEN_response_is_NOT_NULL()
            {
                var response = await _systemUnderTest.UpdatePdfAsync(@"path/to/file.pdf");

                Assert.True(response != null);
            }
        }
    }
}
