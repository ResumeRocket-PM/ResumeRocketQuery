using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Services
{
    public class JobService : IJobService
    {
        private readonly IResumeRocketQueryRepository _resumeRocketQueryRepository;
        private readonly ILanguageService _languageService;

        public JobService(IResumeRocketQueryRepository resumeRocketQueryRepository, ILanguageService languageService)
        {
            _resumeRocketQueryRepository = resumeRocketQueryRepository;
            _languageService = languageService;
        }

        public async Task<int> CreateJobResumeAsync(Job job)
        {
            JobResult jobResult = await _languageService.CaptureJobPostingAsync(job.JobUrl);

            //Nate Put logic here.
            //Parse the Pdf bytes from the Job Object.
            //Take the Text of the Resume
            //Pass it to the language model, with the keywords and description from the Job Posting and ask the language model what changes would be good to make
            //Store this as part of the ResumeContent dictionary.
            string reccomendations = "";

            var resumeContent = job.Resume;

            resumeContent.Add("Reccomendations", reccomendations);

            var result = await _resumeRocketQueryRepository.CreateResumeAsync(new Resume
            {
                AccountID = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = jobResult.CompanyName,
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
            //throw new NotImplementedException();
        }
    }
}
