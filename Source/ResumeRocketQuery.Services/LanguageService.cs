using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IOpenAiClient openAiClient;
        private readonly IJobScraper jobScraper;
        private readonly IJobService jobService;
        private readonly IPdfToHtmlClient pdfToHtmlClient;
        private readonly ILlamaClient _llamaClient;

        public LanguageService(IOpenAiClient openAiClient, IJobScraper jobScraper, ILlamaClient llamaClient, IJobService jobService, IPdfToHtmlClient pdfToHtmlClient)
        {
            this.openAiClient = openAiClient;
            this.jobScraper = jobScraper;
            this.jobService = jobService;
            this.pdfToHtmlClient = pdfToHtmlClient;
            _llamaClient = llamaClient;
        }

        //Create a Method that takes in a URL.
        public async Task<JobResult> CaptureJobPostingAsync(string url)
        {
            jobScraper.ScrapeSetup(url);

            var htmlBody = await jobScraper.ScrapeJobPosting("//html");

            return await ProcessJobPosting(htmlBody, url);
        }
        //Pass the Prompt that we've created, and the HTML into the External class.
        //Deserialize the result as our JobResult class.
        public async Task<JobResult> ProcessJobPosting(string html, string url)
        {
            string siteName = ParseSiteName(url);

            Stream htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            var strippedHtml = await pdfToHtmlClient.StripText(htmlStream);

            if (await jobService.GetJobPostingAsync(url) == null)
            {
                await jobService.StoreJobPostingAsync(url, siteName, strippedHtml);
            }

            var prompt = @"
                    For the provided job posting HTML below, pull the following fields from the job posting. If they aren't found, leave them as null:

                    * Name of the Company posting this job application
                    * the title of the position detailed in the job posting
                    * Date published which  can be nullable
                    * A 1 paragraph TLDR of the job posting description
                    * The top 10 keywords of the job posting
                    * A few perks of the job from the posting, if there are any otherwise an empty list
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

                    Your return content will only contain the requested JSON, no additional text.
                    Additionally, there will be no formatting applied to the return content like
                    markdown code block syntax.

                    The following input is the job posting html

                    {{$input}}";


            var result = await openAiClient.SendMessageAsync(prompt, strippedHtml);

            result = FormatPreCheck(result);

            return JsonConvert.DeserializeObject<JobResult>(result);
        }

        private static string ParseSiteName(string url)
        {
            url = Regex.Replace(url, @"\.com.*", "");

            return url;
        }


        /// <summary>
        ///     Checks for and strips of markdown code block syntax in the event ChatGPT ignores the request to not include it
        /// </summary>
        /// <param name="json"> the returned JSON from ChatGPT </param>
        /// <returns> The plain text version </returns>
        public static string FormatPreCheck(string json)
        {
            json = Regex.Replace(json, @"^```json", "");
            json = Regex.Replace(json, @"$```", "");
            return json;
        }
    }
}
