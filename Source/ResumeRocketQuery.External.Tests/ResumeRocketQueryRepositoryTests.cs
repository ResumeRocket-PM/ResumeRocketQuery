using System;
using System.Threading.Tasks;
using ExpectedObjects;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResumeRocketQuery.Domain.External;
using System.IO;
using Json.More;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ResumeRocketQuery.Repository.Tests
{
    public class OpenAiClientTests
    {
        private IOpenAiClient _systemUnderTest;

        public OpenAiClientTests()
        {
            var serviceProvider = (new ResumeRocketQueryServiceProvider()).Create();

            _systemUnderTest = serviceProvider.GetService<IOpenAiClient>();
        }

        public class SendMessageAsync : OpenAiClientTests
        {
            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_THEN_response_is_NOT_NULL()
            {
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    One line TLDR with the fewest words.",
                    @"
                    1st Law of Thermodynamics - Energy cannot be created or destroyed.
                    2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
                    3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.");

                Assert.True(response != null);
            }

            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_on_true_THEN_returned_true()
            {
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    State whether the given equality is true or false in one word.",
                    @"
                    2+2=4");

                Assert.Equal("true", response.ToLower());
            }

            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_on_false_THEN_returned_false()
            {
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    State whether the given equality is true or false in one word.",
                    @"
                    2+2=6");

                Assert.Equal("false", response.ToLower());
            }
        }
    }
}
