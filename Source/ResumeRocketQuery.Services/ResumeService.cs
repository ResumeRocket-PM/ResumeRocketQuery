using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using System.Diagnostics;

namespace ResumeRocketQuery.Services
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeDataLayer _resumeDataLayer;
        
        public ResumeService(IResumeDataLayer resumeDataLayer)
        {
            _resumeDataLayer = resumeDataLayer;
        }

        public async Task<List<ResumeResult>> GetResumeHistory(int originalResumeId) {
            var result = await _resumeDataLayer.GetResumeHistoryAsync(originalResumeId);
            return result;
        }

        public async Task<ResumeResult> GetResume(int resumeId) {
            var result = await _resumeDataLayer.GetResumeAsync(resumeId);
            return result;
        }

        public async Task<bool> UpdateResume(ResumeStorage resume) {
            try {
                await _resumeDataLayer.UpdateResumeAsync(resume);
                return true;
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                return false;
            }

        }
    }
}