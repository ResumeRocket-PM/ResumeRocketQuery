using System.Linq;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.DataLayer;
using Module.Memory;
using MySqlConnector;
using ResumeRocketQuery.Storage;
using System.Collections.Generic;
using System.Data;
using ResumeRocketQuery.Domain.Services;

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

        public async Task<int> InsertPortfolioStorageAsync(PortfolioStorage portfolio)
        {
            var result = await _memoryStorage.InsertAsync(portfolio, (x, id) => { x.PortfolioId = id; });

            return result;
        }

        public async Task<PortfolioStorage> SelectPortfolioStorageAsync(int accountId)
        {
            var emailAddressStorages = await _memoryStorage.SelectAsync<PortfolioStorage>();

            var result = emailAddressStorages.FirstOrDefault(x => x.AccountId == accountId);

            return result;
        }

        // for resume table
        public async Task<int> InsertResumeStorageAsync(ResumeStorage resume)
        {
            var result = await _memoryStorage.InsertAsync(resume, (x, id) => { x.ResumeID = id; });

            return result;
        }

        public async Task<List<ResumeStorage>> SelectResumeStoragesAsync(int accountID)
        {

            var resumeStorages = await _memoryStorage.SelectAsync<ResumeStorage>();

            var result = resumeStorages.Where(x => x.accountID == accountID);

            return result.ToList();
        }

        public async Task<ResumeStorage> SelectResumeStorageAsync(int resumeId)
        {
            var resumeStorages = await _memoryStorage.SelectAsync<ResumeStorage>();

            var result = resumeStorages.FirstOrDefault(x => x.ResumeID == resumeId);

            return result;
        }
    }
}
