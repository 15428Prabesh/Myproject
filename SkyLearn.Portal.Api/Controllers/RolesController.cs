using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using static Application.Models.Roles;
using System.Net;
using System.Transactions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Core;
using Newtonsoft.Json;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //  [TypeFilter(typeof(AuthorizedUser))]
    public class RolesController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly RolesService _rolesService;
        private readonly PermissionService _permissionService;
        private readonly ActionPermissionService actionPermissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RolesController(APIResponse aPIResponse, ILogger<RolesController> _logger, IMapper mapper, RolesService rolesService, PermissionService permissionService, ActionPermissionService actionPermissionService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _rolesService = rolesService;
            _permissionService = permissionService;
            this.actionPermissionService = actionPermissionService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<RolesDTO>>> GetAllRoles(bool paginate, int pageSize = 10, int pageNumber = 1, string keyword = "")
        {
            try
            {
                var data = await _rolesService.List<Application.Models.Roles, RolesDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Roles list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<RolesDTO>> CreateRole(CreateRoleDTO fields)
        {
            //CreateRoleDTO fields = JsonConvert.DeserializeObject<CreateRoleDTO>(sfields);
            if (fields == null)
                return this.OnBadRequest($"Data is null", "validation", (int)HttpStatusCode.ExpectationFailed);
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                        return this.OnBadRequest("Please enter all the required fields", "validation", 400);
                    if (_rolesService.Exists(fields.RoleName))
                    {
                        return this.OnBadRequest("Role name with name" + fields.RoleName.ToString() + "already exists", "duplicate", 400);
                    }
                    var model = new Roles
                    {
                        RoleName = fields.RoleName,
                        Pid = AppHelper.GeneratePid(Constant.PREFIX_ROLE),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = CurrentUserName
                    };
                    string uniqueKey = nameof(Roles.RoleName);
                    var role = await _rolesService.Create<Application.Models.Roles>(model, uniqueKey, model.RoleName);
                    if (role.Id == 0)
                        return this.OnBadRequest("Duplicate " + uniqueKey, HttpStatusCode.Conflict.ToString(), (int)HttpStatusCode.Conflict);
                    if (fields.PermissionsPid.Count > 0)
                    {
                        foreach (var permissionId in fields.PermissionsPid)
                        {
                            try
                            {
                                var permission = await _permissionService.Retrieve<ControllerAction>(permissionId);
                                var actionPermission = new ActionPermission
                                {
                                    Pid = AppHelper.GeneratePid(Constant.PREFIX_ACTION_PERMISSION),
                                    ControllerAction = permission,
                                    Roles = role,
                                    CreatedBy = "superuser",
                                    CreatedAt = DateTime.UtcNow
                                };

                                await this.actionPermissionService.Create(actionPermission);
                            }
                            catch (Exception ex)
                            {
                                scope.Dispose(); // Rollback the transaction
                                return this.OnBadRequest($"Invalid Permission ID {permissionId}", "validation", 400);
                            }
                        }
                    }

                    var result = _mapper.Map<RolesDTO>(role);
                    scope.Complete();
                    return this.OnSuccess(role, 200);
                }
                catch (Exception ex)
                {
                    this._logger.LogError("Error while creating a Role. {0}", ex);
                    return StatusCode(500, "Internal server error");
                }
            }
        }


        [HttpGet("{Pid}")]
        public async Task<ActionResult<RolesDTO>> GetDetails(string Pid)
        {
            try
            {
                Application.Models.Roles data = await _rolesService.Retrieve<Application.Models.Roles>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<RolesDTO>(data);
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
        public async Task<ActionResult<RolesDTO>> UpdateRole(string Pid, [FromBody] JsonPatchDocument<Roles> fields, [FromQuery] List<string> permissionsPid)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var data = await _rolesService.Retrieve<Roles>(Pid);
                    if (data != null)
                    {
                        if (ModelState.IsValid)
                        {
                            fields.Replace(x => x.Pid, Pid);
                            fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                            fields.Replace(x => x.IsModified, true);
                            if (permissionsPid.Count > 0)
                            {
                                try
                                {
                                    var deletedPermissions = await this._rolesService.DeletePermissions(data.Pid);
                                    if (deletedPermissions)
                                    {
                                        foreach (var permissionId in permissionsPid)
                                        {
                                            var permission = await _permissionService.Retrieve<ControllerAction>(permissionId);
                                            var actionPermission = new ActionPermission
                                            {
                                                Pid = AppHelper.GeneratePid(Constant.PREFIX_ACTION_PERMISSION),
                                                ControllerAction = permission,
                                                Roles = data,
                                                CreatedBy = "superuser",
                                                CreatedAt = DateTime.UtcNow
                                            };

                                            await this.actionPermissionService.Create(actionPermission);
                                        }
                                    }
                                    else
                                    {
                                        scope.Dispose();
                                        return this.OnBadRequest("Failed to delete permissions during update", "validation", 400);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    scope.Dispose();
                                    return this.OnBadRequest($"Invalid Permission ID during update {ex.Message}", "validation", 400);
                                }
                            }

                            var result = _mapper.Map<RolesDTO>(data);
                            scope.Complete();
                            return this.OnSuccess(result, 200);
                        }
                        else
                        {
                            return this.OnBadRequest($"Invalid update data for role with Pid {Pid}", "validation", 400);
                        }
                    }
                    else
                    {
                        return this.OnBadRequest($"Invalid update data for role with Pid {Pid}", "validation", 400);
                    }
                }
                catch (Exception ex)
                {

                    this._logger.LogError("Error while updating a Role. {0}", ex);
                    return StatusCode(500, "Internal server error");
                }
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteRole(string Pid)
        {
            try
            {
                var instance = await _rolesService.Retrieve<Application.Models.Roles>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Role Not Found", "not found", 404);
                }
                var res = await _rolesService.Delete(instance, false);
                return this.OnSuccess("Role Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Role {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
