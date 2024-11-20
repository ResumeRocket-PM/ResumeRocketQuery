using Newtonsoft.Json;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.Services
{
    public class JobService : IJobService
    {
        private readonly IJobDataLayer _jobDataLayer;

        public JobService(IJobDataLayer jobDataLayer)
        {
            _jobDataLayer = jobDataLayer;
        }

        public async Task StoreJobPostingAsync(string url, string company, string posting)
        {
            JobPosting job = new JobPosting
            {
                JobUrl = url,
                JobCompany = company,
                JobDescription = posting
            };
            await _jobDataLayer.StoreJobPostingAsync(job);
            return;
        }

        public async Task<JobPosting> GetJobPostingAsync(string url)
        {
            return await _jobDataLayer.GetJobPostingAsync(url);
        }
    }
}