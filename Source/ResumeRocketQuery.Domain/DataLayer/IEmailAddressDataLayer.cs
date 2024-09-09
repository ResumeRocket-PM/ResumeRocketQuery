using ResumeRocketQuery.Domain.Services.Repository;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IEmailAddressDataLayer
    {
        Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddress);
        Task UpdateEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage);
        Task<EmailAddressRepository> GetEmailAddressAsync(int emailAddressId);
    }
}
