using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Services;
using SkyLearn.Portal.Api.Middleware;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class FormController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly FormService _formService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FormController(APIResponse aPIResponse, ILogger<FormController> _logger, IMapper mapper, FormService formService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _formService = formService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<FormDTO>>> GetAllForms(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _formService.List<Application.Models.Form,FormDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Forms list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<FormDTO>> CreateForm(FormDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = _mapper.Map<Application.Models.Form>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_FORM);
                    model.CreatedAt = DateTime.UtcNow;

                    var res = _mapper.Map<FormDTO>(await _formService.Create<Application.Models.Form>(model));
                    return this.OnSuccess(res, 200);
                }
                return this.OnBadRequest("Please enter all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating a Form . {0}", ex);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<FormDTO>> GetDetails(string Pid)
        {
            try
            {
                Application.Models.Form data = await _formService.Retrieve<Application.Models.Form>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<FormDTO>(data);
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
        public async Task<ActionResult<FormDTO>> UpdateForm(string Pid, JsonPatchDocument<Application.Models.Form> fields)
        {
            try
            {
                var data = await _formService.Retrieve<Application.Models.Form>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        fields.Replace(x => x.Pid, Pid);
                        fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                        fields.Replace(x => x.IsModified, true);
                        var res = _mapper.Map<FormDTO>(await _formService.Update(Pid, fields));
                        return this.OnSuccess(res, 200);
                    }
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Form {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteForm(string Pid)
        {
            try
            {
                var instance = await _formService.Retrieve<Application.Models.Form>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Form Not Found", "not found", 404);
                }
                var res = await _formService.Delete(instance, false);
                return this.OnSuccess("Form Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Form {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
