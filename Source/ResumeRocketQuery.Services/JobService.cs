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
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ResumeRocketQuery.Services
{
    public class JobService : IJobService
    {
        private readonly IPdfService pdfService;
        private readonly IOpenAiClient openAiClient;
        private readonly IResumeRocketQueryRepository _resumeRocketQueryRepository;
        private readonly ILanguageService _languageService;

        public JobService(IPdfService pdfService, IOpenAiClient openAiClient, IResumeRocketQueryRepository resumeRocketQueryRepository, ILanguageService languageService)
        {
            this.pdfService = pdfService;
            this.openAiClient = openAiClient;
            _resumeRocketQueryRepository = resumeRocketQueryRepository;
            _languageService = languageService;
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
            var pdfText = await pdfService.ReadPdfAsync(pdf);
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

                Your output should be plain text JSON (no markdown code block syntax) with 4 keys. 
                The first key is 'suggestions' which has a value of a JSON array listing the suggestions. 

                The second key is 'html' which has a string value. This string contains all of the HTML to be used to construct 
                a new resume using the suggested changes.

                The third key is 'css' which has a string value containing the CSS to format the HTML from the second key.

                The last key is 'changes' which will have a value of a JSON array with fives items corresponding to the suggestions key, 
                each item details the changes made to the original resume.

                {{$input}}";

            //Store this as part of the ResumeContent dictionary.
            string response = await openAiClient.SendMessageAsync(prompt, pdfText);
            var recommendations = "";

            try
            {
                var jsonResult = JsonConvert.DeserializeObject<List<String>>(response);
                recommendations = jsonResult[0];
            }
            catch (Exception e) {
                Debug.WriteLine("Error parsing returned JSON", e);
                throw e;
            }

            

            var resumeContent = job.Resume;

            resumeContent.Add("Reccomendations", recommendations);

            var regex = new Regex("https?:\\/\\/([^\\/]+)").Match(job.JobUrl).Groups[1].Value;

            var result = await _resumeRocketQueryRepository.CreateResumeAsync(new Resume
            {
                AccountID = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = regex,
                JobUrl = job.JobUrl,
                Position = jobResult.Title,
                ResumeContent = resumeContent,
                Status = "Pending"
            });

            return result;
        }

        public async Task<List<Resume>> GetResumes(int accountId)
        {
            return await _resumeRocketQueryRepository.GetResumesAsync(accountId);
        }

        public async Task<Resume> GetResume(int resumeId)
        {
            return await _resumeRocketQueryRepository.GetResumeAsync(resumeId);
        }

        public async Task UpdateResume(int resumeId, string status)
        {
            //Resume resume = await _resumeRocketQueryRepository.GetResumeAsync(resumeId);
            ResumeStorage resumeStorage = new();
            resumeStorage.status = status;
            resumeStorage.ResumeID = resumeId;
            await _resumeRocketQueryRepository.UpdateResume(resumeStorage);
        }
    }
}
