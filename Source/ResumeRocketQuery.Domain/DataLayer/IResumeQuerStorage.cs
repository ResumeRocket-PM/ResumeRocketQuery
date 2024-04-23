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
        Task<int> InsertResume(ResumeStorage resume);
        Task<List<ResumeStorage>> SelectResumeStorageAsync(int accountID);
    }
}
