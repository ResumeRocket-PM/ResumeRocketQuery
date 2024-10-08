using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface ISearchDataLayer
    {
        Task<List<SearchResult>> SearchAsync(string searchTerm, int resultCount);
    }
}
