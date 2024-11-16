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
using Microsoft.OpenApi.Validations.Rules;

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
            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
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

            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
            public async Task WHEN_SendMessageAsync_is_called_on_true_THEN_returned_true()
            {
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    State whether the given equality is true or false in one word without any punctuation.",
                    @"
                    2+2=4");

                Assert.Equal("true", response.ToLower());
            }

            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
            public async Task WHEN_SendMessageAsync_is_called_in_succession_THEN_chat_history_logged()
            {
                List<string> prompts = new List<string>();
                prompts.Add(
                    @"Remember the following for this for this chat:

                     * Until I send the keyword ""Respond"" by itself, you are only allowed to respond with ""...""
                     * That includes in response to this message"
                );
                prompts.Add(@"Using the result of the expression: 18+2");
                prompts.Add(@"compute the value of the of that value divide by 2.");
                prompts.Add(@"provide the answer as a single numeric value, no other text.");
                prompts.Add(@"Respond");

                var response = await _systemUnderTest.SendMultiMessageAsync(prompts);

                Assert.Equal("10", response.ToLower());
            }

            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
            public async Task WHEN_SendMessageAsync_is_called_on_false_THEN_returned_false()
            {
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    State whether the given equality is true or false in one word, without any punctuation.",
                    @"
                    2+2=6");

                Assert.Equal("false", response.ToLower());
            }

            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
            public async Task WHEN_SendMessageAsync_is_called_on_marketstar_page_source_THEN_assert_keywords()
            {
                var jobPosting = File.ReadAllText(@"./Samples/MarketStar/MarketStar.html");
                var response = await _systemUnderTest.SendMessageAsync(
                    @"{{$input}} 

                    For the provided webpage source code for a job posting, return a JSON array where every element is a string. 
                    Those elements will be the top 10 keywords that can used to update a resume. There should be no other text in
                    the response, only the JSON text which is in plain text, not markdown code block syntax.",
                    jobPosting);

                var jsonResult = JsonConvert.DeserializeObject<List<String>>(response);

                Debug.WriteLine(jsonResult);

                Assert.IsType<List<String>>(jsonResult);
                Assert.Equal(10, jsonResult.Count);
            }

            // [Fact]
            [Fact(Skip = "Skipping to conserve API credits")]
            public static void WHEN_JSON()
            {
                string json =
                    @"[
                        {
                          ""section"": ""Skills"",
                          ""original"": ""Programming: Java, HTML, CSS, C++, SQL, C#, Python, PHP, bash, PowerShell, JavaScript"",
                          ""modified"": ""SQL, Python, JavaScript, Data Visualization (Tableau), Data Analysis, System Development Life Cycle (SDLC), Team Collaboration""
                        },
                        {
                          ""section"": ""Projects"",
                          ""original"": ""Lead the design and construction of a physical database, constructed with MySQL on AWS servers"",
                          ""modified"": ""Designed relational databases and processed large datasets using SQL and Python""
                        },
                        {
                          ""section"": ""Work Experience"",
                          ""original"": ""Performed internal audits for the Utah site in 2020 and 2021 for both ISO 27001 and PCI"",
                          ""modified"": ""Collaborated with cross-functional teams to present data insights for improving processes""
                        },
                        {
                          ""section"": ""Projects"",
                          ""original"": ""Created an entire design document for a proposed dating app including mock interfaces, feasibility matrices"",
                          ""modified"": ""Collaborated with team members to analyze data and present insights using data visualization tools""
                        },
                        {
                          ""section"": ""Work Experience"",
                          ""original"": ""Routinely audited physical spare and operational inventory to validate records"",
                          ""modified"": ""Utilized SQL and Python for data querying, automation, and process optimization""
                        }
                      ]
                    ";

                var result = JsonConvert.DeserializeObject<List<Change>>(json);
            }
        }
    }
}
