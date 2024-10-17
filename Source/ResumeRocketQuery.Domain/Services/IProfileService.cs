using ResumeRocketQuery.Domain.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.Services
{
    public interface IProfileService
    {
        Task<List<ProfileResult>> GetUniversity(string input, bool orderType = true);
        Task<List<ProfileResult>> GetState(string input, bool orderType = true);
        Task<List<ProfileResult>> GetMajor(string input, bool orderType = true);
        Task<List<ProfileResult>> GetCareer(string input, bool orderType = true);

    }
}
