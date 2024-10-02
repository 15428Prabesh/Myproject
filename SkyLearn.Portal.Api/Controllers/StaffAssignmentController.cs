using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.Portal.Api.Interfaces;
using SkyLearn.Portal.Api.Middleware;
using SkyLearn.Portal.Api.Services;
using System.Net;

namespace SkyLearn.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class StaffAssignmentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        private readonly AssignmentService _assignmentService;
        public StaffAssignmentController(APIResponse aPIResponse, ILogger<StudentController> _logger, IMapper mapper, IAssignmentEnrollService assignmentEnrollService, AssignmentService assignmentService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentEnrollService = assignmentEnrollService;
            _httpContextAccessor = httpContextAccessor;
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AssignmentDTO>>> GetAllAssignments(bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            try
            {
                var data = await _assignmentService.List<Assignment, AssignmentDTO>(paginate, pageSize, pageNumber);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignments {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<AssignmentDTO>> GetDetails(string Pid)
        {
            try
            {
                Assignment data = await _assignmentService.Retrieve<Assignment>(Pid);
                if (data != null)
                {
                    var res = _mapper.Map<AssignmentDetailDTO>(data);
                    return this.OnSuccess(res, (int)HttpStatusCode.OK);
                }
                return this.OnNotFound("Data not found", "error", (int)HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving details . {0}", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveAssignment(string id, string remarks)
        {
            var data = await _assignmentEnrollService.ApproveAssignment(id, remarks, CurrentUserID);
            if (data.Data > 0)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Assignment approve successfully.");
               
            }
            else if (data.Data == -1)
            {
                return this.OnBadRequest("Assignment is in proccess.You cannot approve this assignemnt.","validation" ,(int)HttpStatusCode.BadRequest);
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);               

            }           
        }

        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectAssignment(string id, string remarks)
        {
            var data = await _assignmentEnrollService.RejectAssignment(id, remarks, CurrentUserID);

            if (data.Data > 0)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Assignment rejected successfully.");
            }
            else if (data.Data == -1)
            {
                return this.OnBadRequest("Assignment is in proccess.You cannot reject this assignemnt.", "validation", (int)HttpStatusCode.BadRequest);
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);

            }

        }        

        [HttpGet("{id}/student/assignment/list")]
        public async Task<IActionResult> GetStaffStudentAssignementList(string id,string? searchText,string? status, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetStaffStudentAssignementList(id, searchText, status,pageSize,pageNumber, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("student/assignment/list")]
        public async Task<IActionResult> GetStudentAssignementList(string? searchText, string? status, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetAllStaffAssignment(pageSize, pageNumber,searchText,status, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("student/assignment/{id}")]
        public async Task<IActionResult> GetAssignementDetail(string id)
        {
            var data = await _assignmentEnrollService.GetAssignementDetail(id, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{assignemtID}/student/{id}")]
        public async Task<IActionResult> GetStudentAssignementDetail(string assignemtID, string id)
        {
            var data = await _assignmentEnrollService.GetAssignementDetail( id, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }


        [HttpGet("{assignemtID}/student/{id}/logs")]
        public async Task<IActionResult> GetStaffStudentAssignementLogList(string assignemtID, string id, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetStaffStudentAssignementLogList(assignemtID, id, pageNumber, pageSize, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("status/list")]
        public async Task<IActionResult> GetStatus()
        {
            var data = await _assignmentEnrollService.GetStatusList();
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }
    }
}
