using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Services.Repository;

namespace ResumeRocketQuery.Services
{
    public class ProfileService: IProfileService
    {
        private readonly IProfileDataLayer _profileDataLayer;
        public ProfileService(IProfileDataLayer profileDataLayer) 
        {
            _profileDataLayer = profileDataLayer;
        }

        /// <summary>
        /// get the val stored in DB.University
        /// 
        /// 'varToken' should be the column names that you want to get, and you could enter:
        /// (1) name: university name
        /// (2) area: university located area(state or city)
        /// (3) contry: university located contry(US or not US)
        /// 
        /// 'input' should be the user input value, the search method would search the entries
        /// in DB based on the input, if you wanna show all list, just provide an empty string ''
        /// 
        /// 'orderType' is binary say you wanna ascending order or descending order, the default is ascending
        /// 
        /// </summary>
        /// <param name="varToken"></param>
        /// <param name="input"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public async Task<List<ProfileResult>> GetUniversity(string input, bool orderType = true)
        {
            var result = await _profileDataLayer.SearchUniversityNameAsync(input, orderType);

            return result;
        }

        public async Task<List<ProfileResult>> GetState(string input, bool orderType = true)
        {
            var result = await _profileDataLayer.SearchStateNameAsync(input, orderType);

            return result;
        }

        public async Task<List<ProfileResult>> GetMajor(string input, bool orderType = true)
        {
            var result = await _profileDataLayer.SearchMajorNameAsync(input, orderType);

            return result;
        }

        public async Task<List<ProfileResult>> GetCareer(string input, bool orderType = true)
        {
            var result = await _profileDataLayer.SearchCareerNameAsync(input, orderType);

            return result;
        }
    }
}
