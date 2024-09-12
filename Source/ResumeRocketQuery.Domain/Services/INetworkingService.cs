using System.Collections.Generic;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Domain.Services;

public interface INetworkingService
{
    Task<List<AccountDetails>> FilterAccounts(string filter, string searchterm);
}