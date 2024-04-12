using Newtonsoft.Json;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IOpenAiClient openAiClient;
        private readonly IJobScraper jobScraper;

        public LanguageService(IOpenAiClient openAiClient, IJobScraper jobScraper)
        {
            this.openAiClient = openAiClient;
            this.jobScraper = jobScraper;
        }

        //Create a Method that takes in a URL.
        public async Task<JobResult> CaptureJobPostingAsync(string url)
        {
            var htmlBody = await jobScraper.scrapJobPosting(url);

            var prompt = @"
                          For the same job posting source code provided, pull the following information from the job posting:

                            * Job posting title
                            * Date published which  can be nullable
                            * A 1 paragraph TLDR of the job posting description
                            * The top 10 keywords of the job posting
                            * A few perks of the job from the posting
                            * A list  of the base job requirements

                            Arrange the returned data in a JSON object following this format:

                            public class JobPosting:
                            {
                                public string Title { get; set; }
                                public DateTime DatePosted { get; set; }
                                public string Description { get; set; }
                                public List<string> Keywords { get; set; }
                                public List<string> Perks { get; set; }
                                public List<string> Requirements { get; set; }
                            }";

            var result = await openAiClient.SendMessageAsync(prompt, htmlBody);

            return JsonConvert.DeserializeObject<JobResult>(result);
        }
        //Pass the Prompt that we've created, and the HTML into the External class.
        //Deserialize the result as our JobResult class.
    }
}
