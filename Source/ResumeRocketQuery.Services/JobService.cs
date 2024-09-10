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

namespace ResumeRocketQuery.Services
{
    public class JobService : IJobService
    {
        private readonly IPdfService _pdfService;
        private readonly IOpenAiClient _openAiClient;
        private readonly IResumeDataLayer _resumeRocketQueryRepository;
        private readonly ILanguageService _languageService;

        public JobService(IPdfService pdfService, 
            IOpenAiClient openAiClient, 
            ILanguageService languageService)
        {
            _pdfService = pdfService;
            _openAiClient = openAiClient;
            _languageService = languageService;
        }

        public async Task<int> CreateJobResumeAsync(Job job)
        {
            JobResult jobResult = await _languageService.CaptureJobPostingAsync(job.JobUrl);

            //Nate Put logic here.
            //Parse the Pdf bytes from the Job Object.
            var pdfBytes = Convert.FromBase64String(job.Resume["FileBytes"]);
            var pdf = new MemoryStream(pdfBytes);
            //Take the Text of the Resume
            var pdfText = await _pdfService.ReadPdfAsync(pdf);
            //Pass it to the language model, with the keywords and description from the Job Posting and ask the language model what changes would be good to make

            var prompt = $"Based on this job description: \r\n\r\n<DESCRIPTION BEGINS>{jobResult.Description}<DESCRIPTION ENDS>\r\n\r\n " +
                         $"and these 10 keywords selected from the job posting\n\n<DESCRIPTION BEGINS>{jobResult.Keywords}<DESCRIPTION ENDS>\n\n " +
                         "Provide a list of suggestions, separated by the new line character: '\n', on what needs to be changed for the following resume content\n\n{{$input}}";

            //Store this as part of the ResumeContent dictionary.
            string reccomendations = await _openAiClient.SendMessageAsync(prompt, pdfText);

            var resumeContent = job.Resume;

            resumeContent.Add("Reccomendations", reccomendations);

            var regex = new Regex("https?:\\/\\/([^\\/]+)").Match(job.JobUrl).Groups[1].Value;

            throw new NotImplementedException();


            //var result = await _resumeRocketQueryRepository.CreateResumeAsync(new Resume
            //{
            //    AccountID = job.AccountId,
            //    ApplyDate = DateTime.Now,
            //    CompanyName = regex,
            //    JobUrl = job.JobUrl,
            //    Position = jobResult.Title,
            //    ResumeContent = resumeContent,
            //    Status = "Pending"
            //});


            //return result;
        }

        public async Task<List<Resume>> GetResumes(int accountId)
        {
            //return await _resumeRocketQueryRepository.GetResumesAsync(accountId);
            throw new NotImplementedException();
        }

        public async Task<Resume> GetResume(int resumeId)
        {
            //return await _resumeRocketQueryRepository.GetResumeAsync(resumeId);

            throw new NotImplementedException();
        }

        public async Task UpdateResume(int resumeId, string status)
        {

            throw new NotImplementedException();
            //Resume resume = await _resumeRocketQueryRepository.GetResumeAsync(resumeId);
            //ResumeStorage resumeStorage = new();
            //resumeStorage.status = status;
            //resumeStorage.ResumeID = resumeId;
            //await _resumeRocketQueryRepository.UpdateResume(resumeStorage);
            //throw new NotImplementedException();
        }
    }
}
