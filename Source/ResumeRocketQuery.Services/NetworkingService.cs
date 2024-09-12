using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iText.Layout.Element;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Services
{
    public class NetworkingService : INetworkingService
    {
        private readonly IAccountDataLayer _accountDataLayer;
        private readonly IAccountService _accountService;

        public NetworkingService(IAccountDataLayer accountDataLayer, 
            IAccountService accountService
            )
        {
            _accountDataLayer = accountDataLayer;
            _accountService = accountService;
        }
        public async Task<List<AccountDetails>> FilterAccounts(string filter, string searchterm)
        {
            var accounts = await _accountDataLayer.SelectAccountStoragesByFilterAsync(filter, searchterm);

            List<AccountDetails> result = new List<AccountDetails>();

            List<Task> tasks = new List<Task>();

            foreach (var account in accounts)
            {
                var task = Task.Run(async () =>
                {
                    var details = await _accountService.GetAccountAsync(account.AccountId);

                    result.Add(details);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return result;
        }
    }
}
