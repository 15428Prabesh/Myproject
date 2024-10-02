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
    public class AdminDashboardController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentService _assignmentService;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        private readonly CoursesService _coursesService;
        private readonly DepartmentService _departmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminDashboardController(APIResponse aPIResponse, ILogger<AssignmentController> _logger, IMapper mapper, AssignmentService assignmentService, CoursesService coursesService, DepartmentService departmentService, IAssignmentEnrollService assignmentEnrollService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentService = assignmentService;
            _coursesService = coursesService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
            _assignmentEnrollService=assignmentEnrollService;
        }

        [HttpGet("assignment/recent")]
        public async Task<IActionResult> GetAlltudentRecentAssignment()
        {
            var data = await _assignmentEnrollService.GetAllAdminStudentAssignment(1, 5, "","", CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ResponseModel<StudentAssignmentSummaryDTO>>> GetStudentAssignmentSummary()
        {
            var data = await _assignmentEnrollService.GetAdminStudentAssignementSummary(CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }
    }
}
