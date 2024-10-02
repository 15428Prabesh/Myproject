using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Models;
using SkyLearn.Portal.Api.Services;
using Application.BaseManager;
using System.Net;
using Application.Response;
using Application.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class UserRolesController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly UserRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRolesController(APIResponse aPIResponse, ILogger<UserRolesController> logger, UserRoleService userRoleService,IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, logger, httpContextAccessor)
        {
            _mapper = mapper;
            _roleService = userRoleService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<ActionResult<UserRoleDTO>> CreateUserRole(CreateUserRoleDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = _mapper.Map<UserRole>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_USERROLE);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = "superuser";
                    var data = _mapper.Map<UserRoleDTO>(await _roleService.Create(model));
                    return this.OnSuccess(data, 200);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating User Roles.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<UserRoleDTO>>> GetAllUserRoles(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _roleService.List<UserRole,UserRoleDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching UserRoles {0}", ex);
                throw new Exception("An error occurred while fetching UserRoles", ex);
            }
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<UserRoleDTO>> GetDetails(string Pid)
        {
            try
            {
                UserRole data = await _roleService.Retrieve<UserRole>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<UserRoleDTO>(data);
                    return this.OnSuccess(dto_data, 200);
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving data . {0}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{Pid}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(string Pid, JsonPatchDocument<UserRole> fields)
        {
            try
            {
                var data = await _roleService.Retrieve<UserRole>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        fields.Replace(x => x.Pid, Pid);
                        fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                        fields.Replace(x => x.IsModified, true);
                        var res = _mapper.Map<UserDTO>(await _roleService.Update(Pid, fields));
                        return this.OnSuccess(res, 200);
                    }
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating User {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteUser(string Pid)
        {
            try
            {
                var instance = await _roleService.Retrieve<UserRole>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("User Not Found", "not found", 404);
                }
                var res = await _roleService.Delete(instance, false);
                return this.OnSuccess("UserRole Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting UserRole {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
