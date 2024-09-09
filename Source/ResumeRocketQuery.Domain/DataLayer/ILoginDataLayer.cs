using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface ILoginDataLayer
    {
        Task<int> InsertLoginStorageAsync(LoginStorage login);
        Task UpdateLoginStorageAsync(LoginStorage loginStorage);
        Task<Login> GetLoginAsync(int accountId);
    }
}
