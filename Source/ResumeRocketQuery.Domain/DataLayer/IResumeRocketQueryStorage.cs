using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IResumeRocketQueryStorage
    {
        Task<int> InsertAccountStorageAsync(AccountStorage accountStorage);
        Task<AccountStorage> SelectAccountStorageAsync(int accountId);
        Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage);
        Task<EmailAddressStorage> SelectEmailAddressStorageByEmailAddressAsync(string emailAddress);
        Task<EmailAddressStorage> SelectEmailAddressStorageAsync(int emailAddressId);
        Task<EmailAddressStorage> SelectEmailAddressStorageByAccountIdAsync(int accountId);
        Task<int> InsertPortfolioStorageAsync(PortfolioStorage portfolio);
        Task<PortfolioStorage> SelectPortfolioStorageAsync(int accountId);

        Task<int> InsertResume(ResumeStorage resume);
        Task<List<ResumeStorage>> SelectResumeStorageAsync(int accountID);
    }
}
