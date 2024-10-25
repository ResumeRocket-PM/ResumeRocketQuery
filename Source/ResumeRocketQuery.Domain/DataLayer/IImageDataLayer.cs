using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public interface IImageDataLayer
    {
        Task<int> InsertImageRecordAsync(string imageUrl, string fileName);
    }
}
