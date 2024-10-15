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
        public async Task<List<ProfileResult>> SearchStateNameAsync(string sName, bool isAsc = true) 
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                if (isAsc)
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchStatesNameASC,
                                new
                                {
                                    stateName = $"%{sName}%"
                                    //orderType = oType
                                },
                                commandType: CommandType.Text);

                    return result.ToList(); 
                }
                else
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchStatesNameDESC,
                                new
                                {
                                    stateName = $"%{sName}%"
                                    //orderType = oType
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
            }
        }

        public async Task<List<ProfileResult>> SearchUniversityNameAsync(string univName, bool isAsc)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                if (isAsc)
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchUniversityNameASC,
                                new
                                {
                                    uName = $"%{univName}%"
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
                else
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchUniversityNameDESC,
                                new
                                {
                                    uName = $"%{univName}%"
                                    //orderType = oType
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
            }
        }

        public async Task<List<ProfileResult>> SearchCareerNameAsync(string careerName, bool isAsc)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                if (isAsc)
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchCareerNameASC,
                                new
                                {
                                    cName = $"%{careerName}%"
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
                else
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchCareerNameDESC,
                                new
                                {
                                    cName = $"%{careerName}%"
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
            }
        }

        public async Task<List<ProfileResult>> SearchMajorNameAsync(string majorName, bool isAsc)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                if (isAsc)
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchMajorNameASC,
                                new
                                {
                                    mName = $"%{majorName}%"
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
                else
                {
                    var result = await connection.QueryAsync<ProfileResult>(
                                DataLayerConstants.StoredProcedures.Profile.SearchMajorNameDESC,
                                new
                                {
                                    mName = $"%{majorName}%"
                                },
                                commandType: CommandType.Text);

                    return result.ToList();
                }
            }
        }

        // show states

        // show career

        // show major
    }
}
