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
    public class StudentAssignmentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        public StudentAssignmentController(APIResponse aPIResponse, ILogger<StudentController> _logger, IMapper mapper, IAssignmentEnrollService assignmentEnrollService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentEnrollService = assignmentEnrollService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("submit/{id}")]
        public async Task<IActionResult> SubmitAssignment(string id, SubmissionInfo submission)
        {
            var data = await _assignmentEnrollService.SubmitAssignment(id, submission, CurrentUserID);           

            if (data.Data > 0)
            {
                return this.OnSuccess(data, (int)HttpStatusCode.OK, "Assignment submitted successfully.");
            }
            else if (data.Data == -1)
            {
                return this.OnBadRequest("Assignment is in proccess.You cannot submitted this assignemnt.", "validation", (int)HttpStatusCode.BadRequest);
            }
            else
            {
                return this.OnBadRequest("Error occured in system.Please contact administrator.", "internal server error", (int)HttpStatusCode.BadRequest);

            }            
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudentAssignment(string? searchText,string? status, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetAllStudentAssignment(pageNumber, pageSize, searchText,status, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentAssignementDetail(string id)
        {
            var data = await _assignmentEnrollService.GetAssignementDetail(id, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetSTudentAssignementLogList(string id, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetSTudentAssignementLogList(id,pageSize, pageNumber, CurrentUserID);
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
