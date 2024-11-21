﻿using System.Threading.Tasks;
using System.Collections.Generic;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Api.Request;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IResumeService
    {
        Task<int> CreatePrimaryResume(ResumeRequest request);
        //Task<int> CreateResume(ResumeRequest resume);
        //Task<ResumeResult> CreateResumeFromPdf(ResumeRequest request);
        Task<int> CreateResumeFromPdf(ResumeRequest request);
        Task<int> CreateResumeFromHtml(ResumeRequest request, string resumeHtml);
        Task<string> GetPrimaryResume(int accountId);
        Task<byte[]> GetPrimaryResumePdf(int accountId);
        Task<string> GetResume(int resumeId);
        Task<byte[]> GetResumePdf(int resumeId);
        Task<byte[]> GetResumePdfFromHtml(string html);
        Task<List<ResumeStorage>> GetResumeHistory(int originalResumeId);
        Task<bool> UpdateResume(ResumeStorage resume);
        Task<List<ResumeStorage>> GetAccountResumes(int accountId);
        Task<GetResumeResult> GetPerfectResume(int resumeId, int applicationId);
        Task ApplyResumeSuggestions(List<SuggestionStatus> suggestionStatuses);
    }
}
