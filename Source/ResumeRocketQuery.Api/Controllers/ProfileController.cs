using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Api.Request;
using ResumeRocketQuery.Domain.Api.Response;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Repository;
using ResumeRocketQuery.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ResumeRocketQuery.Api.Controllers
{
    [Authorize]
    [Route("api/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IServiceResponseBuilder _serviceResponseBuilder;
        private readonly IResumeRocketQueryUserBuilder _resumeRocketQueryUserBuilder;
        private readonly IProfileService _profileService;
        public ProfileController(IServiceResponseBuilder serviceResponseBuilder,
            IResumeRocketQueryUserBuilder resumeRocketQueryUserBuilder,
            IProfileService profileService)
        {
            _serviceResponseBuilder = serviceResponseBuilder;
            _resumeRocketQueryUserBuilder = resumeRocketQueryUserBuilder;
            _profileService = profileService;
        }

        [HttpGet]
        [Route("UniversityName/{name}/{ordertype}")]
        public async Task<ServiceResponseGeneric<List<string>>> GetUniversityName([FromRoute]string name, [FromRoute] bool ordertype)
        {
            var universityList = await _profileService.GetUniversity(name, ordertype);

            List<string> uNameList = new List<string>();
            if (universityList is not null)
            {
                foreach (var u in universityList)
                {
                    uNameList.Add(u.UniversityName);
                }
                var response = _serviceResponseBuilder.BuildServiceResponse(uNameList, HttpStatusCode.OK);
                return response;
            }
            return _serviceResponseBuilder.BuildServiceResponse<List<string>>(null, HttpStatusCode.NotFound);

        }

        [HttpGet]
        [Route("StateName/{name}/{ordertype}")]
        public async Task<ServiceResponseGeneric<List<string>>> GetStateName([FromRoute] string name, [FromRoute] bool ordertype)
        {
            var stateNameList = await _profileService.GetState(name, ordertype);

            List<string> sNameList = new List<string>();
            if (stateNameList is not null)
            {
                foreach (var s in stateNameList)
                {
                    sNameList.Add(s.StatesName);
                }
                return _serviceResponseBuilder.BuildServiceResponse(sNameList, HttpStatusCode.OK);
            }
            return _serviceResponseBuilder.BuildServiceResponse<List<string>>(null, HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("StateCode/{code}/{ordertype}")]
        public async Task<ServiceResponseGeneric<List<string>>> GetStateCode([FromRoute] string code, [FromRoute] bool ordertype)
        {
            var stateCodeList = await _profileService.GetState(code, ordertype);

            List<string> sCodeList = new List<string>();
            if (stateCodeList is not null)
            {
                foreach (var s in stateCodeList)
                {
                    sCodeList.Add(s.StateCode);
                }
                return _serviceResponseBuilder.BuildServiceResponse(sCodeList, HttpStatusCode.OK);
            }
            return _serviceResponseBuilder.BuildServiceResponse<List<string>>(null, HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("MajorName/{name}/{ordertype}")]
        public async Task<ServiceResponseGeneric<List<string>>> GetMajorName([FromRoute] string name, [FromRoute] bool ordertype)
        {
            var majorList = await _profileService.GetMajor(name, ordertype);

            List<string> mList = new List<string>();
            if (majorList is not null)
            {
                foreach (var m in majorList)
                {
                    mList.Add(m.MajorName);
                }
                return _serviceResponseBuilder.BuildServiceResponse(mList, HttpStatusCode.OK);
            }
            return _serviceResponseBuilder.BuildServiceResponse<List<string>>(null, HttpStatusCode.NotFound);
        }

        [HttpGet]
        [Route("careerName/{name}/{ordertype}")]
        public async Task<ServiceResponseGeneric<List<string>>> GetCareerName([FromRoute] string name, [FromRoute] bool ordertype)
        {
            var careerList = await _profileService.GetCareer(name, ordertype);

            List<string> cList = new List<string>();
            if (careerList is not null)
            {
                foreach (var c in careerList)
                {
                    cList.Add(c.CareerName);
                }
                return _serviceResponseBuilder.BuildServiceResponse(cList, HttpStatusCode.OK);
            }
            return _serviceResponseBuilder.BuildServiceResponse<List<string>>(null, HttpStatusCode.NotFound);
        }
    }
}
