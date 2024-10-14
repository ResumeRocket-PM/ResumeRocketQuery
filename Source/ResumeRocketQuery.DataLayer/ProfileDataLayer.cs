using System;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Domain.DataLayer;

namespace ResumeRocketQuery.DataLayer
{
    public class ProfileDataLayer: IProfileDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ProfileDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }
        /// <summary>
        /// search the university name by the given name
        /// </summary>
        /// <returns></returns>
        public async Task<List<SearchResult>> SearchStateNameAsync(string sName, bool isAsc = true) 
        {
            string oType = "ASC";
            if (!isAsc)
            {
                oType = "DESC";
            }

            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.QueryAsync<SearchResult>(
                    DataLayerConstants.StoredProcedures.Profile.SearchStatesName,
                    new
                    {
                        stateNames = sName,
                        orderType = oType
                    },
                    commandType: CommandType.Text);

                return result.ToList();
            }
        }
        // show states

        // show career

        // show major
    }
}
