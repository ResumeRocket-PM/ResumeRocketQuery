using System.Linq;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using Module.Memory;

namespace ResumeRocketQuery.Tests.Fakes
{
    public class MemoryResumeRocketQueryStorage : IResumeRocketQueryStorage
    {
        private readonly MemoryStorage _memoryStorage;

        public MemoryResumeRocketQueryStorage()
        {
            _memoryStorage = new MemoryStorage();
        }

        public async Task<int> InsertAccountStorageAsync(AccountStorage accountStorage)
        {
            var result = await _memoryStorage.InsertAsync(accountStorage, (x, id) => { x.AccountId = id; });

            return result;
        }

        public async Task<AccountStorage> SelectAccountStorageAsync(int accountId)
        {
            var emailAddressStorages = await _memoryStorage.SelectAsync<AccountStorage>();

            var result = emailAddressStorages.FirstOrDefault(x => x.AccountId == accountId);

            return result;
        }

        public async Task<int> InsertEmailAddressStorageAsync(EmailAddressStorage emailAddressStorage)
        {
            var result = await _memoryStorage.InsertAsync(emailAddressStorage, (x, id) => { x.EmailAddressId = id; });

            return result;
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByEmailAddressAsync(string emailAddress)
        {
            var emailAddressStorages = await _memoryStorage.SelectAsync<EmailAddressStorage>();

            var result = emailAddressStorages.FirstOrDefault(x => x.EmailAddress == emailAddress);

            return result;
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageAsync(int emailAddressId)
        {
            var emailAddressStorages = await _memoryStorage.SelectAsync<EmailAddressStorage>();

            var result = emailAddressStorages.FirstOrDefault(x => x.EmailAddressId == emailAddressId);

            return result;
        }

        public async Task<EmailAddressStorage> SelectEmailAddressStorageByAccountIdAsync(int accountId)
        {
            var emailAddressStorages = await _memoryStorage.SelectAsync<EmailAddressStorage>();

            var result = emailAddressStorages.FirstOrDefault(x => x.AccountId == accountId);

            return result;
        }
    }
}
