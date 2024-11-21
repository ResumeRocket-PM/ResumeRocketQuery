using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services;

public interface IExtensionService
{
    Task<bool> IsJobApplication(string html);
    Task<string> CreateHtmlQueryForEmbeddingButton(string site, string html);
    Task<decimal> GenerateProbabilityMatchAgainstJob(string html, int resumeId);
    string GenerateApplyButtonXPath(string site);
}