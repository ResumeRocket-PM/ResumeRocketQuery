using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IProfileDataLayer
    {
        Task<List<SearchResult>> SearchStateNameAsync(string sName, bool isAsc);
    }
}
