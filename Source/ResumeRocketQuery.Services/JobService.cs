using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Service;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ResumeRocketQuery.Services
{
    public class JobService : IJobService
    {
        private readonly IPdfService _pdfService;
        private readonly IOpenAiClient _openAiClient;
        private readonly IResumeDataLayer _resumeDataLayer;
        private readonly IApplicationDataLayer _applicationDataLayer;
        private readonly ILanguageService _languageService;

        public JobService(IPdfService pdfService, 
            IOpenAiClient openAiClient, 
            ILanguageService languageService,
            IResumeDataLayer resumeDataLayer,
            IApplicationDataLayer applicationDataLayer)
        {
            _pdfService = pdfService;
            _openAiClient = openAiClient;
            _languageService = languageService;
            _resumeDataLayer = resumeDataLayer;
            _applicationDataLayer = applicationDataLayer;
        }

        public async Task<int> CreateJobResumeAsync(Job job)
        {
            JobResult jobResult = await _languageService.CaptureJobPostingAsync(job.JobUrl);

            //Nate Put logic here.
            //Parse the Pdf bytes from the Job Object.
            var pdfBytes = Convert.FromBase64String(job.Resume["FileBytes"]);
            var pdf = new MemoryStream(pdfBytes);
            System.IO.File.WriteAllBytes("hello.pdf", pdfBytes);
            //Take the Text of the Resume
            var pdfText = await _pdfService.ReadPdfAsync(pdf);
            //Pass it to the language model, with the keywords and description from the Job Posting and ask the language model what changes would be good to make

            //var prompt = $"Based on this job description: \r\n\r\n<DESCRIPTION BEGINS>{jobResult.Description}<DESCRIPTION ENDS>\r\n\r\n " +
            //             $"and these 10 keywords selected from the job posting\n\n<DESCRIPTION BEGINS>{jobResult.Keywords}<DESCRIPTION ENDS>\n\n " +
            //             "Provide a list of suggestions, separated by the new line character: '\n', on what needs to be changed for the following resume content\n\n{{$input}}";

            /// TODO - Using the attached resume as a reference along with the 5 suggestions you made from that job posting, given the content in the resume apply the 5 suggestions for updates to the resume.



        var prompt = 
                $@"Using this input resume content, along with this job description:

                {jobResult.Description}

                and these 10 keywords:

                {jobResult.Keywords}

                produce 5 suggestiobs for changes that should be made to the resume, and apply them to the resume.

                These updates should not falsify any information, meaning no additional skills, education, or work experience 
                should be added you are only allowed to reword items on the resume to better match the job posting.

                Your output should be plain text JSON (no markdown code block syntax) with 3 keys. 

                The first key is 'html' which has a string value. This string contains all of the HTML to be used to construct 
                a new resume using the suggested changes.

                The second key is 'css' which has a string value containing the CSS to format the HTML from the second key.

                The last key is 'changes' which will have a value of a JSON array with fives items corresponding to the suggestions key, 
                each item will have the following JSON array item structure: a key for ""section"" with string value specifying the section
                the original resume that is being addressed, a key of ""original"" with string value that is the original content that was on
                the resume, and a key for ""modified"" with string value of the suggested change to the ""original"" text.

                {{$input}}";

            //Store this as part of the ResumeContent dictionary.
            string response = await _openAiClient.SendMessageAsync(prompt, pdfText);
            var recommendations = new List<Change>();
            var html = "";
            var css = "";

            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Updates>(response);
                html = jsonResult.html;
                css = jsonResult.css;
                recommendations = jsonResult.changes;

            }
            catch (Exception e) {
                Debug.WriteLine("Error parsing returned JSON", e);
                throw e;
            }

            var newPDF = _pdfService.CreatePdfAsync(job.Resume["FileName"], html, css);

            job.Resume["FileBytes"] = File.ReadAllBytes(newPDF.ToString()).ToString(); ;
            var resumeContent = job.Resume;

            //resumeContent.Add("Recommendations", recommendations); //replace key FileBytes with new Pdf bytes

            var regex = new Regex("https?:\\/\\/([^\\/]+)").Match(job.JobUrl).Groups[1].Value;

            var resumeId = await _resumeDataLayer.InsertResumeAsync(new ResumeStorage
            {
                Resume = JsonConvert.SerializeObject(resumeContent),
                AccountId = job.AccountId,
            });

            var result = await _applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
            {
                JobPostingUrl = job.JobUrl,
                AccountId = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = regex,
                Position = jobResult.Title,
                Status = "Pending",
                ResumeId = resumeId
            });

            return result;
        }

        public async Task<List<ApplicationResult>> GetJobPostings(int accountId)
        {
            var applications = await _applicationDataLayer.GetApplicationsAsync(accountId);


            List<ApplicationResult> result = new List<ApplicationResult>();

            foreach (var application in applications)
            {
                var applicationResult = await ConvertApplication(application);

                result.Add(applicationResult);
            }

            return result;
        }

        public async Task<ApplicationResult> GetResume(int resumeId)
        {
            var application = await _applicationDataLayer.GetApplicationAsync(resumeId);

            var result = await ConvertApplication(application);

            return result;
        }

        public async Task UpdateResume(int resumeId, string status)
        {
            await _applicationDataLayer.UpdateApplicationAsync(new ApplicationStorage
            {
                ApplicationId = resumeId,
                Status = status
            });
        }

        private async Task<ApplicationResult> ConvertApplication(Application x)
        {
            var resumeContent = new Dictionary<string, string>();

            if (x.ResumeId.HasValue)
            {
                var resumeResult = await _resumeDataLayer.GetResumeAsync(x.ResumeId.Value);

                resumeContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(resumeResult.Resume);
            }

            return new ApplicationResult
            {
                CompanyName = x.CompanyName,
                AccountID = x.AccountId,
                ApplyDate = x.ApplyDate,
                JobUrl = string.Empty,
                Position = x.Position,
                ResumeID = x.ApplicationId,
                Status = x.Status,
                ResumeContent = resumeContent
            };
        }
    }
}
