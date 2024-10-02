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
    public class ResultController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly ResultService _resultService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ResultController(APIResponse aPIResponse, ILogger<ResultController> _logger, IMapper mapper, ResultService resultService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _resultService = resultService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<ResultDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await _resultService.Retrieve<Result>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Result", "validation", 400);
                }
                var data = _mapper.Map<ResultDTO>(entity);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Result details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<ResultDTO>>> GetAllResult(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _resultService.List<Result,ResultDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Result {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<ResultDTO>> CreateResult(CreateResultDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = _mapper.Map<Result>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_RESULT);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = "superuser";
                    var data = _mapper.Map<ResultDTO>(await _resultService.Create(model));
                    return this.OnSuccess(data, 200);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Result.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("{Pid}")]
        public async Task<ActionResult<ResultDTO>> UpdateResult(string Pid, JsonPatchDocument<Result> fields)
        {
            try
            {
                var data = await _resultService.Retrieve<Result>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        fields.Replace(x => x.Pid, Pid);
                        fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                        fields.Replace(x => x.IsModified, true);
                        var res = _mapper.Map<ResultDTO>(await _resultService.Update(Pid, fields));
                        return this.OnSuccess(res, 200);
                    }
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Result {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteResult(string Pid)
        {
            try
            {
                var instance = await _resultService.Retrieve<Result>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Result Not Found", "not found", 404);
                }
                var res = await _resultService.Delete(instance, false);
                return this.OnSuccess("Result Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Result {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
