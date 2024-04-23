using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IResumeQueryStorage
    {
        // for resume
        Task<string> InsertResume( string url, string resume);
        Task<string> SelectResumeStorageAsync(string Url, int accountID);
    }
}
