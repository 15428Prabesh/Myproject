using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Core.Constants;
using Core.Helper.APiCall;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Net;
using NuGet.Protocol;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
   // [TypeFilter(typeof(AuthorizedUser))]
    public class AuthController : BaseAPIController
    {
        private readonly BaseUrl _baseUrl;
        private UserRoleService userRoleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(APIResponse aPIResponse, IOptions<BaseUrl> baseUrl, ILogger<AuthController> _logger, IConfiguration configuration,UserRoleService userRoleService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _baseUrl = baseUrl.Value;
            this.userRoleService = userRoleService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost("login")]
        public async Task<ActionResult<List<AssignmentDTO>>> UserLogin(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Origin", Request.Headers.Origin.ToString());
                    ApiClient ? apiClient = new ApiClient();
                    LoginResonse? apiResponse = await apiClient.GetAsyncResult<LoginResonse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.LoginUrl, HttpMethod.Post, headers, login);
                    if (apiResponse!=null && apiResponse.status == HttpStatusCode.OK)
                    {   
                        var data = apiResponse.data;                        
                        return this.OnSuccess(data, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignments {0}", ex);
                return this.OnBadRequest("Invalid username or password", "validation", (int)HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("verify-user")]
        public async Task<ActionResult> EmailVerify([Required]string token)
        {   
            if(!String.IsNullOrEmpty(token))
            {   
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Origin", Request.Headers.Origin.ToString());
                ApiClient? apiClient=new ApiClient();
                LoginResonse? apiResponse = await apiClient.GetAsyncResult<LoginResonse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.EmailValidation+"?token="+token, HttpMethod.Post, headers);
                if (apiResponse.status == HttpStatusCode.OK)
                    {                           
                        return this.OnSuccess("Email Verified Successfully",(int)HttpStatusCode.OK);
                    }
                return this.OnBadRequest("Invalid Token","error",(int)HttpStatusCode.BadRequest);
            }
            return this.OnBadRequest("Field cannot be empty","validation",(int)HttpStatusCode.BadRequest);
        }
    }
}
