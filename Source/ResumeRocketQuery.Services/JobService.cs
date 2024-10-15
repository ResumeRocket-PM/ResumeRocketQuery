using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
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
        private readonly IPdfToHtmlClient _pdfToHtmlClient;
        private readonly ILanguageService _languageService;

        public JobService(IPdfService pdfService, 
            IOpenAiClient openAiClient, 
            ILanguageService languageService,
            IResumeDataLayer resumeDataLayer,
            IApplicationDataLayer applicationDataLayer,
            IPdfToHtmlClient pdfToHtmlClient)
        {
            _pdfService = pdfService;
            _openAiClient = openAiClient;
            _languageService = languageService;
            _resumeDataLayer = resumeDataLayer;
            _applicationDataLayer = applicationDataLayer;
            _pdfToHtmlClient = pdfToHtmlClient;
        }

        public async Task<int> CreateJobResumeAsync(Job job)
        {
            JobResult jobResult = await _languageService.CaptureJobPostingAsync(job.JobUrl);

            //Parse the Pdf bytes from the Job Object.
            var pdfBytes = Convert.FromBase64String(job.Resume["FileBytes"]);
            var pdf = new MemoryStream(pdfBytes);
            
            // Take the Text of the Resume
            var htmlStream = await _pdfToHtmlClient.ConvertPdf(pdf);
            var originalHtml = "";
            //Store this as part of the ResumeContent dictionary.
            using (StreamReader reader = new StreamReader(htmlStream))
            {
                originalHtml = reader.ReadToEnd();
            }

            //Pass it to the language model, with the keywords and description from the Job Posting and ask the language model what changes would be good to make
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
                    a new resume using the suggested changes. The name of the stylesheet in the link to the css style sheet is style.css.

                    The second key is 'css' which has a string value containing the CSS to format the HTML from the second key.

                    The last key is 'changes' which will have a value of a JSON array with fives items corresponding to the suggestions key, 
                    each item will have the following JSON array item structure: a key for ""section"" with string value specifying the section
                    the original resume that is being addressed, a key of ""original"" with string value that is the original content that was on
                    the resume, and a key for ""modified"" with string value of the suggested change to the ""original"" text.

                    {{$input}}";

            
            string response = await _openAiClient.SendMessageAsync(prompt, originalHtml);
            var recommendations = new List<Change>();
            var newHtml = "";
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<Updates>(response);
                newHtml = jsonResult.html;
                recommendations = jsonResult.changes;
            }
            catch (Exception e) {
                Debug.WriteLine("Error parsing returned JSON", e);
                throw;
            }

            var originalResumeId = await _resumeDataLayer.InsertResumeAsync(new ResumeStorage
            {
                AccountId = job.AccountId,
                Resume = originalHtml,
                OriginalResumeID = null,
                OriginalResume = true,
                Version = 1
            });

            var newResumeId = await _resumeDataLayer.InsertResumeAsync(new ResumeStorage
            {
                AccountId = job.AccountId,
                Resume = newHtml,
                OriginalResumeID = originalResumeId,
                OriginalResume = false,
                Version = 2
            });

            // job.Resume.Add("Recommendations", recommendations.ToString());

            var regex = new Regex("https?:\\/\\/([^\\/]+)").Match(job.JobUrl).Groups[1].Value;

            var result = await _applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
            {
                JobPostingUrl = job.JobUrl,
                AccountId = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = regex,
                Position = jobResult.Title,
                Status = "Pending",
                ResumeId = newResumeId
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

        public async Task<ApplicationResult> GetApplication(int applicationId)
        {
            var application = await _applicationDataLayer.GetApplicationAsync(applicationId);

            var result = await ConvertApplication(application);

            return result;
        }

        public async Task<ApplicationResult> GetResumeHistory(int resumeId)
        {
            var application = await _applicationDataLayer.GetApplicationAsync(resumeId);

            var result = await ConvertApplication(application);

            return result;
        }

        public async Task UpdateApplication(int applicationId, string status)
        {
            await _applicationDataLayer.UpdateApplicationAsync(new ApplicationStorage
            {
                ApplicationId = applicationId,
                Status = status
            });
        }

        private async Task<ApplicationResult> ConvertApplication(Application x)
        {
            var resumeContent = "";

            if (x.ResumeId.HasValue)
            {
                var resumeResult = await _resumeDataLayer.GetResumeAsync(x.ResumeId.Value);

                resumeContent = resumeResult.Resume;
            }

            return new ApplicationResult
            {
                CompanyName = x.CompanyName,
                AccountID = x.AccountId,
                ApplyDate = x.ApplyDate,
                JobUrl = x.JobPostingUrl,
                Position = x.Position,
                ResumeID = x.ApplicationId,
                Status = x.Status,
                ResumeContent = resumeContent
            };
        }
    }
}
