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
using System.Collections.Generic;
using Newtonsoft.Json;

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

            [Fact]
            public async Task WHEN_SendMessageAsync_is_called_on_marketstar_page_source_THEN_assert_keywords()
            {
                var jobPosting = File.ReadAllText(@".\Samples\MarketStar\MarketStar.html");
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    For the provided webpage source code for a job posting, return a JSON array where every element is a string. Those elements should be the top 10 keywords I can use to update my resume with.",
                    jobPosting);

                var jsonResult = JsonConvert.DeserializeObject<List<String>>(response);

                Debug.WriteLine(jsonResult);

                Assert.IsType<List<String>>(jsonResult);
                Assert.Equal(10, jsonResult.Count);
            }

            // TODO - uncomment test after web scraper is truncating the source code
            /*[Fact]
            public async Task WHEN_SendMessageAsync_is_called_on_1800contacts_page_source_THEN_assert_keywords()
            {
                var jobPosting = File.ReadAllText(@"C:\Users\Azira\git\resumerocketquery\Source\ResumeRocketQuery.External.Tests\Samples\1800Contacts\1800Contacts.html");
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    For the provided webpage source code for a job posting, return a JSON array where every element is a string. Those elements should be the top 10 keywords I can use to update my resume with.",
                    jobPosting);

                var jsonResult = JsonConvert.DeserializeObject<List<String>>(response);

                Debug.WriteLine(jsonResult);

                Assert.IsType<List<String>>(jsonResult);
                Assert.Equal(10, jsonResult.Count);
            }*/
        }
    }
}
