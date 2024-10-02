using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Security.Cryptography;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class ComponentController : BaseAPIController
    {       
        private readonly ComponentService componentService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ComponentController(APIResponse aPIResponse, ILogger<ComponentController> _logger, ComponentService restService, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger,httpContextAccessor)
        {
            componentService = restService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<ActionResult<ComponentDTO>> CreateComponent(ComponentDTO componentView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var comp_data = _mapper.Map<Component>(componentView);
                    comp_data.Pid = AppHelper.GeneratePid(Constant.PREFIX_COMPONENT);
                    comp_data.CreatedAt = DateTime.UtcNow;
                    comp_data.CreatedBy = "superuser";
                    var data = _mapper.Map<ComponentDTO>(await componentService.Create(comp_data));
                    return this.OnSuccess(data, 200);
                }
                return this.OnBadRequest("Fill all the required forms", "validation");
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating component.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<ComponentDTO>> GetDetail(string Pid)
        {
            try
            {

                var res = await componentService.Retrieve<Component>(Pid);

                if (res == null)
                {
                    return this.OnBadRequest("Invalid component pid", "validation", 400);
                }
                var data = _mapper.Map<ComponentDTO>(res);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining component details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("ByName/{Name}")]
        public async Task<ActionResult<ComponentDTO>> GetComponentByName(string Name)
        {
            try
            {

                var component = await componentService.RetrieveByName<Component>(Name);

                if (component == null)
                {
                    return this.OnNotFound("Component not found", "error", 404);
                }

                var res = _mapper.Map<ComponentDTO>(component);
                return this.OnSuccess(res);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining component {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
