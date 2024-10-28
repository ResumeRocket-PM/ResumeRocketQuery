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
        private readonly IOpenAiClient _openAiClient;
        private readonly IResumeDataLayer _resumeDataLayer;
        private readonly IApplicationDataLayer _applicationDataLayer;
        private readonly IPdfToHtmlClient _pdfToHtmlClient;
        private readonly IResumeService _resumeService;
        private readonly ILanguageService _languageService;

        public JobService(IOpenAiClient openAiClient, 
            ILanguageService languageService,
            IResumeDataLayer resumeDataLayer,
            IApplicationDataLayer applicationDataLayer,
            IPdfToHtmlClient pdfToHtmlClient,
            IResumeService resumeService)
        {
            _openAiClient = openAiClient;
            _languageService = languageService;
            _resumeDataLayer = resumeDataLayer;
            _applicationDataLayer = applicationDataLayer;
            _pdfToHtmlClient = pdfToHtmlClient;
            _resumeService = resumeService;
        }

        public async Task<int> CreateJobAsync(ApplicationRequest applicationRequest)
        {
            //Take the HTML from the Posting, Pass
            var jobResult = await _languageService.ProcessJobPosting(applicationRequest.JobHtml);
            var primaryResume = await _resumeService.GetPrimaryResume(applicationRequest.AccountId);
            var prompt = GeneratePrompt(jobResult.Description, jobResult.Keywords);
            string response = await _openAiClient.SendMessageAsync(prompt, primaryResume);

            try {
                var jsonResult = JsonConvert.DeserializeObject<List<Change>>(response);
            }
            catch (Exception e) {
                Debug.WriteLine("Error parsing returned JSON", e);
                throw;
            }

            var originalResumeId = await _resumeDataLayer.InsertResumeAsync(new ResumeStorage
            {
                AccountId = applicationRequest.AccountId,
                Resume = primaryResume,
                OriginalResumeID = null,
                OriginalResume = true,
                Version = 1
            });

            var result = await _applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
            {
                JobPostingUrl = applicationRequest.JobUrl,
                AccountId = applicationRequest.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = jobResult.CompanyName,
                Position = jobResult.Title,
                Status = "Pending",
                ResumeId = originalResumeId
            });

            return result;
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

            var prompt = GeneratePrompt(jobResult.Description, jobResult.Keywords);
            string response = await _openAiClient.SendMessageAsync(prompt, originalHtml);
            var recommendations = new List<Change>();
            
            try
            {
                var jsonResult = JsonConvert.DeserializeObject<List<Change>>(response);
                recommendations = jsonResult;
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
                Version = 1,
                InsertDate = DateTime.Today,
                UpdateDate = DateTime.Today
            });





            var regex = new Regex("https?:\\/\\/([^\\/]+)").Match(job.JobUrl).Groups[1].Value;

            var result = await _applicationDataLayer.InsertApplicationAsync(new ApplicationStorage
            {
                JobPostingUrl = job.JobUrl,
                AccountId = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = regex,
                Position = jobResult.Title,
                Status = "Pending",
                ResumeId = originalResumeId
            });
            return result;
        }

        private string GeneratePrompt(string description, List<string> keywords)
        {
            //Pass it to the language model, with the keywords and description from the Job Posting and ask the language model what changes would be good to make
            var prompt =
                $@"Using this input resume content, along with this job description:

                    {description}

                    and these 10 keywords:

                    {string.Join(", ", keywords)}

                    produce 5 suggestions for changes that should be made to the resume.

                    These updates should not falsify any information, meaning no additional skills, education, or work experience 
                    should be added you are only allowed to reword items on the resume that are synonyms for items in the job posting
                     to better match the job posting.

                    Your output should be plain text JSON (no markdown code block syntax).

                    The returned JSON will be an array with five JSON objects corresponding to the suggestions, each item will have the following 
                    JSON array item structure: a key of ""original"" with string value that is the exact (word for word) original content that is on
                    the resume, a key for ""modified"" with string value of the suggested change to the ""original"" text, and a key for ""explanation"" 
                    with a short 2-3 sentence string value specifying why the change was suggested.

                    {{{{$input}}}}";

            return prompt;
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
            int? resumeId = null;

            if (x.ResumeId.HasValue)
            {
                var resumeResult = await _resumeDataLayer.GetResumeAsync(x.ResumeId.Value);

                resumeContent = resumeResult.Resume;
                resumeId = resumeResult.ResumeId;
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
                ResumeContent = resumeContent,
                ResumeContentId = resumeId
            };
        }
    }
}
