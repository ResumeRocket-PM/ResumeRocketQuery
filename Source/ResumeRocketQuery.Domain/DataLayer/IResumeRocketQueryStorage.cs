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


    }
}
