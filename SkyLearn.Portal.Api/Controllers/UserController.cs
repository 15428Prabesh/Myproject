using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using Core.Helper.APiCall;
using System.Net;
using Core;
using Microsoft.AspNetCore.Cors;
using System.Text;
using Core.Constants;
using Microsoft.Extensions.Options;
using Application.Helpers;
using NuGet.Protocol;
using SkyLearn.Portal.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using SkyLearn.Portal.Api.Interfaces;
using Application;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class UserController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly UsersService _usersService;
        private readonly BaseUrl _baseUrl;
        private RolesService role_services;
        private UserRoleService userRoleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStaffDapperService _staffDapperService;
        private readonly IStudentDapperService _studentDapperService;
        public UserController(APIResponse aPIResponse, IOptions<BaseUrl> baseUrl, ILogger<UserController> _logger, IMapper mapper, UsersService usersService, IConfiguration configuration, RolesService rolesService, UserRoleService userRoleService, IHttpContextAccessor httpContextAccessor, IStudentDapperService studentDapperService, IStaffDapperService staffDapperService) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _usersService = usersService;
            _baseUrl = baseUrl.Value;
            role_services = rolesService;
            this.userRoleService = userRoleService;
            _staffDapperService = staffDapperService;
            _studentDapperService = studentDapperService;
        }


        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetAllUser(int pageNumber = 1, int pageSize = 10, string keyword = "")
        {
            try
            {
                string queryString = string.Format("?pageNumber={0}&pageSize={1}&keyword={2}", pageNumber, pageSize, keyword);
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Origin", Request.Headers.Origin.ToString());
                ApiClient? apiClient = new ApiClient();
                ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.GetUsers + queryString, HttpMethod.Get, headers);
                if (apiResponse.status == HttpStatusCode.OK)
                {
                    var result = apiResponse;
                    return this.OnSuccess(result.data, 200);
                }
                return this.OnBadRequest("Cannot find the user list", "error", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating a User . {0}", ex);
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<UserDTO>> GetDetails(string Pid)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Origin", Request.Headers.Origin.ToString());
                ApiClient? apiClient = new ApiClient();
                UserDetailResponse? apiResponse = await apiClient.GetAsyncResult<UserDetailResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.GetUsers + "/" + Pid, HttpMethod.Get, headers);
                if (apiResponse.status == HttpStatusCode.OK)
                {
                    var result = apiResponse;
                    this._logger.LogInformation(result.data.Idx);
                    var role_group = await userRoleService.GetRoles(result.data.Idx);
                    result.data.RolesPid = role_group;
                    return this.OnSuccess(result.data, 200);
                }
                return this.OnBadRequest("Cannot find the user list", "error", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating a User . {0}", ex);
                return StatusCode(500, ex);
            }
        }

        [HttpPut("{Pid}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(string Pid, UpdateUserDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Origin", Request.Headers.Origin.ToString());
                    ApiClient? apiClient = new ApiClient();
                    ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.GetUsers + "/" + Pid, HttpMethod.Put, headers, fields);
                    if (apiResponse.status == HttpStatusCode.OK)
                    {
                        foreach (var role in fields.RolesPid)
                        {
                            await this.userRoleService.RemoveAllRoles(Pid);
                            var role_instance = await role_services.Retrieve<Roles>(role);
                            if (role_services == null)
                            {
                                return this.OnBadRequest("Invalid Role Pid", "validation");
                            }
                            UserRole userRole = new UserRole
                            {
                                Pid = AppHelper.GeneratePid(Constant.PREFIX_USER_ROLE),
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = "superuser",
                                UserIdx = Pid,
                                Role = role_instance
                            };
                            await this.userRoleService.Create<UserRole>(userRole);

                        }
                        var result = apiResponse;
                        return this.OnSuccess("User Updated successfully", 200);
                    }
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while updating a User . {0}", ex);
                return StatusCode(500, ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult> CreateUsers(CreateUserDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HttpStatusCode statusCode = await UserCreation(fields);
                    if (statusCode == HttpStatusCode.OK)
                    {
                        return this.OnSuccess("User Created succefully");
                    }
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating a User . {0}", ex);
                return StatusCode(500, ex);
            }
        }
        [NonAction]
        public async Task<HttpStatusCode> UserCreation(CreateUserDTO fields)
        {
            IdentityUserDTO identity_model = _mapper.Map<IdentityUserDTO>(fields);
            identity_model.Password = UniqueIndexGenerator.GenerateUniqueAlphanumericIndex(10);
            identity_model.ConfirmPassword = identity_model.Password;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Origin", CurrentOrigin);
            ApiClient? apiClient = new ApiClient();
            ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.GetUsers, HttpMethod.Post, headers, identity_model);
            if (apiResponse != null && apiResponse.status == HttpStatusCode.OK)
            {
                var result = apiResponse.data;
                Users userModel = _mapper.Map<Users>(fields);
                userModel.Idx = result.ToString();
                userModel.CreatedAt = DateTime.Now;
                userModel.UserName = userModel.Email;
                userModel.Pid = UniqueIndexGenerator.GenerateUniqueAlphanumericIndex(10, Constant.PREFIX_USERS);
                userModel.CreatedBy = "superuser";
                await this._usersService.Create<Users>(userModel);

                foreach (var role in fields.RolesPid)
                {

                    var role_instance = await role_services.Retrieve<Roles>(role);
                    if (role_services == null)
                    {
                        return HttpStatusCode.NonAuthoritativeInformation;
                    }
                    UserRole userRole = new UserRole
                    {
                        Pid = AppHelper.GeneratePid(Constant.PREFIX_USER_ROLE),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = CurrentUserName,
                        UserIdx = result.ToString(),
                        Role = role_instance

                    };
                    await this.userRoleService.Create<UserRole>(userRole);
                }
                return HttpStatusCode.OK;
            }
            return HttpStatusCode.BadRequest;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    headers.Add("Origin", Request.Headers.Origin.ToString());
                    ApiClient? apiClient = new ApiClient();
                    ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.ForgotPassword, HttpMethod.Post, headers, fields);
                    if (apiResponse != null && apiResponse.status == HttpStatusCode.OK)
                    {
                        return this.OnSuccess(apiResponse.message, (int)HttpStatusCode.OK);
                    }
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while updating a User . {0}", ex);
                return StatusCode(500, ex);
            }

        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    ApiClient? apiClient = new ApiClient();
                    ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.ResetPassword, HttpMethod.Post, headers, fields);
                    if (apiResponse != null && apiResponse.status == HttpStatusCode.OK)
                    {
                        return this.OnSuccess(apiResponse.message, (int)HttpStatusCode.OK);
                    }
                    else return this.OnBadRequest("Invalid token", "validation", (int)HttpStatusCode.BadRequest);
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while updating a User . {0}", ex);
                return StatusCode(500, ex);
            }

        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    ApiClient? apiClient = new ApiClient();
                    ApiResponse? apiResponse = await apiClient.GetAsyncResult<ApiResponse>(_baseUrl.IdentityBaseUrl + ApiEndPoints.RefreshToken, HttpMethod.Post, headers);
                    if (apiResponse != null && apiResponse.status == HttpStatusCode.OK)
                    {
                        return this.OnSuccess(apiResponse.message, (int)HttpStatusCode.OK);
                    }
                    else return this.OnBadRequest("Invalid token", "validation", (int)HttpStatusCode.BadRequest);
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", (int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while updating a User . {0}", ex);
                return StatusCode(500, ex);
            }

        }
        [HttpGet("profile")]
        public async Task<ActionResult> GetUserProfile()
        {
            string[] parts = CurrentUserID.Split('_');
            BaseDTO dto = DtoFactory.CreateDto(parts[0]);
            if (dto is StaffDTO staffDTO)
            {
                var data = await _staffDapperService.GetByID(CurrentUserID, CurrentUserName);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            else
            {
                var data = await _studentDapperService.GetByID(CurrentUserID, CurrentUserName);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UserProfile userProfile)
        {
            string[] parts = CurrentUserID.Split('_');
            BaseDTO dto = DtoFactory.CreateDto(parts[0]);
            if (dto is StaffDTO staffDTO)
            {
                Staff staff = _mapper.Map<Staff>(userProfile);
                var data = await _staffDapperService.AddUpdate(CurrentUserID, staff, CurrentUserName);
                return this.OnSuccess(data);
            }
            else
            {
                Student student = _mapper.Map<Student>(userProfile);
                var data = await _studentDapperService.AddUpdate(CurrentUserID, student, CurrentUserName);
                return this.OnSuccess(data);
            }
        }

    }
}




