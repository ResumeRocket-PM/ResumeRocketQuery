﻿using ResumeRocketQuery.Domain.DataLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface IApplicationService
{
    Task<int> CreateJobResumeAsync(Job job);
    Task<List<ApplicationResult>> GetJobPostings(int accountId);
    Task<ApplicationResult> GetApplication(int applicationId);
    Task UpdateApplication(int applicationId, string status);
    Task<int> CreateJobAsync(ApplicationRequest applicationRequest);
    string GetResumeText(string html);
}