using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchDataLayer _searchDataLayer;

        public SearchService(ISearchDataLayer searchDataLayer)
        {
            _searchDataLayer = searchDataLayer;
        }

        public async Task<List<SearchResult>> SearchAsync(string searchTerm, int result)
        { 
            if(searchTerm == "")
            {
                return await _searchDataLayer.GetAllUsersAsync();
            }

            return await _searchDataLayer.SearchAsync(searchTerm, result);
        }
    }
}
