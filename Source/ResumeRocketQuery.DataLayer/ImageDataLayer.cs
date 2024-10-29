using Dapper;
using Microsoft.Data.SqlClient;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.DataLayer
{
    public class ImageDataLayer : IImageDataLayer
    {
        private readonly IResumeRocketQueryConfigurationSettings _resumeRocketQueryConfigurationSettings;

        public ImageDataLayer(IResumeRocketQueryConfigurationSettings resumeRocketQueryConfigurationSettings)
        {
            _resumeRocketQueryConfigurationSettings = resumeRocketQueryConfigurationSettings;
        }
        public async Task<int> InsertImageRecordAsync(string imageUrl, string fileName)
        {
            using (var connection = new SqlConnection(_resumeRocketQueryConfigurationSettings.ResumeRocketQueryDatabaseConnectionString))
            {
                var result = await connection.ExecuteScalarAsync<int>(
                    DataLayerConstants.StoredProcedures.Image.InsertImage,
                    new
                    {
                        ImageUrl = imageUrl,
                        FileName = fileName
                    },
                    commandType: CommandType.Text);
                return result;
            }
        }
    }
}
