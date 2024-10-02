using Application.Helpers;
using Application.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using System.Reflection;
using System.Net;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "superadmin, superuser")]
    [TypeFilter(typeof(AuthorizedUser))]
    public class PermissionController : BaseAPIController
    {
        private readonly PermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PermissionController(APIResponse aPIResponse, ILogger<PermissionController> logger, PermissionService permissionService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, logger, httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions(int roleId, string keyword="")
        {
            var permissions = await _permissionService.GetAllPermissions(roleId, keyword);
            return this.OnSuccess(permissions);
        }
        [HttpPost]
        public async Task<ActionResult> UpsertPermission(int roleId, string permissionIds)
        {
            try
            {
                if (roleId <= 0 || string.IsNullOrWhiteSpace(permissionIds))
                {
                    return this.BadRequest("Invalid roleId or PermissionIds.");
                }
                var permissionIdList = permissionIds.Split(',');
                var actionPermissions = new List<ActionPermission>();
                foreach (var permissionId in permissionIdList)
                {
                    var actionPermission = new ActionPermission
                    {
                        RoleId = roleId,
                        Pid = AppHelper.GeneratePid("per"),
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Sushil",
                        IsDeleted = false,
                        ControllerActionId = int.Parse(permissionId)
                    };
                    actionPermissions.Add(actionPermission);
                    // await _permissionService.Create<ActionPermission>(actionPermission);
                }
                if (await _permissionService.UpsertRolePermission(actionPermissions))
                    return this.OnSuccess("", (int)HttpStatusCode.OK);
                else return this.BadRequest(HttpStatusCode.BadRequest);
            }
            catch (FormatException ex)
            {
                return this.BadRequest($"Invalid permissionIds format: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message.ToString());
            }
        }
    }
}
