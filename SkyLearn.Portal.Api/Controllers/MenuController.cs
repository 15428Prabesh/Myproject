using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class MenuController : BaseAPIController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MenuService _menuService;

        public MenuController(APIResponse aPIResponse, ILogger<MenuController> _logger, IMapper mapper, MenuService menuService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _menuService = menuService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<Menu>>> GetMenu()
        {
            try
            {
                List<UserRole> role = (List<UserRole>)_httpContextAccessor.HttpContext.Items["roles"];
                if (role == null)
                    return null;
                List<int> result = new List<int>();
                foreach (UserRole r in role)
                {
                    result.Add(r.RoleId);
                }
                var data = await _menuService.GetMenuByRole(result);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Roles list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
