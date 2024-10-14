using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IResumeService
    {
        Task CreatePrimaryResume(ResumeRequest request);
        Task<ResumeResult> CreateResumeFromPdf(ResumeRequest request);
        Task<string> GetPrimaryResume(int accountId);
        Task<byte[]> GetPrimaryResumePdf(int accountId);
        Task<string> GetResume(int resumeId);
        Task<byte[]> GetResumePdf(int resumeId);
    }
}
