using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var result = await _resumeRocketQueryRepository.CreateResumeAsync(new Resume
            {
                AccountID = job.AccountId,
                ApplyDate = DateTime.Now,
                CompanyName = jobResult.CompanyName,
                JobUrl = job.JobUrl,
                Position = jobResult.Title,
                ResumeContent = job.Resume,
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
    }
}
