using ResumeRocketQuery.Domain.DataLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface ISearchService
    {
        Task<List<SearchResult>> SearchAsync(string searchTerm, int result);
    }
}
