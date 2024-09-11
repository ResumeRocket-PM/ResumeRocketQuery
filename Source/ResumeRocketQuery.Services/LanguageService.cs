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
            jobScraper.ScrapeSetup(url);
            var htmlBody = await jobScraper.ScrapeJobPosting("//html");

            var prompt = @"
                          For the provided job posting HTML below, pull the following fields from the job posting. If they aren't found, leave them as null:

                            * Name of the Company posting this job application
                            * the title of the position detailed in the job posting
                            * Date published which  can be nullable
                            * A 1 paragraph TLDR of the job posting description
                            * The top 10 keywords of the job posting
                            * A few perks of the job from the posting
                            * A list  of the base job requirements

                            Arrange the returned data in a JSON object following this format:

                            public class JobPosting:
                            {
                                public string CompanyName { get; set; }
                                public string Title { get; set; }
                                public string Description { get; set; }
                                public List<string> Keywords { get; set; }
                                public List<string> Perks { get; set; }
                                public List<string> Requirements { get; set; }
                            }

                            The following input is the job posting html

                            {{$input}}";

            var result = await openAiClient.SendMessageAsync(prompt, htmlBody);

            return JsonConvert.DeserializeObject<JobResult>(result);
        }
        //Pass the Prompt that we've created, and the HTML into the External class.
        //Deserialize the result as our JobResult class.
    }
}
