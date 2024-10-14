using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IResumeService
    {
        Task<ResumeResult> CreateResumeFromPdf(ResumeRequest request);
    }
}
