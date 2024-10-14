using ResumeRocketQuery.Domain.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IProfileDataLayer
    {
        Task<List<ProfileResult>> SearchStateNameAsync(string sName, bool isAsc);
        Task<List<ProfileResult>> SearchUniversityNameAsync(string univName, bool isAsc);
        Task<List<ProfileResult>> SearchCareerNameAsync(string careerName, bool isAsc);
        Task<List<ProfileResult>> SearchMajorNameAsync(string majorName, bool isAsc);



    }
}
