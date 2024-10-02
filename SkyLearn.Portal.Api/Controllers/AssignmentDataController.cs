using Application.BaseManager;
using Application.Helpers;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Drawing.Printing;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class AssignmentDataController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentService _assignmentService;
        private readonly AssignmentDataService _assignmentDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AssignmentDataController(APIResponse aPIResponse, ILogger<AssignmentDataController> _logger, IMapper mapper, AssignmentService assignmentService, AssignmentDataService assignmentDataService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentService = assignmentService;
            _assignmentDataService = assignmentDataService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<AssignmentDataDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await _assignmentDataService.Retrieve<AssignmentData>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Assignment", "validation", 400);
                }
                var data = _mapper.Map<AssignmentDataDTO>(entity);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Assignment details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AssignmentDataDTO>> CreateAssignmentData(CreateAssignmentData fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = _mapper.Map<AssignmentData>(fields);
                    model.Pid = AppHelper.GeneratePid(Constant.PREFIX_ASSIGNMENTDATA);
                    model.CreatedAt = DateTime.UtcNow;
                    model.CreatedBy = "superuser";
                    var data = _mapper.Map<AssignmentDataDTO>(await _assignmentDataService.Create(model));
                    return this.OnSuccess(data, 200);
                }
                return this.OnBadRequest("Fill all the required fields", "validation", 400);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating Assignment.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet]
        public async Task<ActionResult<List<AssignmentDataDTO>>> GetAllAssignmentsData(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _assignmentDataService.List<AssignmentData,AssignmentDataDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignments {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPatch("{Pid}")]
        public async Task<ActionResult<AssignmentDataDTO>> UpdateAssignmentData(string Pid, JsonPatchDocument<AssignmentData> fields)
        {
            try
            {
                var data = await _assignmentDataService.Retrieve<AssignmentData>(Pid);
                if (data != null)
                {
                    if (ModelState.IsValid)
                    {
                        fields.Replace(x => x.Pid, Pid);
                        fields.Replace(x => x.UpdatedAt, DateTime.UtcNow);
                        fields.Replace(x => x.IsModified, true);
                        var res = _mapper.Map<AssignmentDataDTO>(await _assignmentDataService.Update(Pid, fields));
                        return this.OnSuccess(res, 200);
                    }
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Updating Assignment {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteAssignmentData(string Pid)
        {
            try
            {
                var instance = await _assignmentService.Retrieve<AssignmentData>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Assignment Not Found", "not found", 404);
                }
                var res = await _assignmentService.Delete(instance, false);
                return this.OnSuccess("Assignment Deleted successfully", 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting Assignment {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }

        }
    }
}
