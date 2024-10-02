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
    public class FormDataController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly FormService _formService;
        private readonly FormDataService _formDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FormDataController(APIResponse aPIResponse, ILogger<FormDataController> _logger, IMapper mapper, FormService formService, FormDataService formDataService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _formService = formService;
            _formDataService = formDataService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<FormDataDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await _formDataService.Retrieve<FormData>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Form", "validation", 400);
                }
                var data = _mapper.Map<FormDataDTO>(entity);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Form details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<FormDataDTO>>> GetAllFormData(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _formDataService.List<FormData,FormDataDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Form {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<FormData>> CreateFormData(CreateFormDataDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = _mapper.Map<FormData>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_FORMDATA);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = "superuser";
                    var data = _mapper.Map<FormDataDTO>(await _formDataService.Create(model));
                    return this.OnSuccess(data, 200);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Form.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("{Pid}")]
        public async Task<ActionResult<FormDataDTO>> UpdateFormData(string Pid, JsonPatchDocument<FormData> fields)
        {
            try
            {
                var data = await _formDataService.Retrieve<FormData>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        fields.Replace(x => x.Pid, Pid);
                        fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                        fields.Replace(x => x.IsModified, true);
                        var res = _mapper.Map<FormDataDTO>(await _formDataService.Update(Pid, fields));
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
        public async Task<ActionResult<string>> DeleteFormData(string Pid)
        {
            try
            {
                var instance = await _formDataService.Retrieve<FormData>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Form Not Found", "not found", 404);
                }
                var res = await _formDataService.Delete(instance, false);
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
