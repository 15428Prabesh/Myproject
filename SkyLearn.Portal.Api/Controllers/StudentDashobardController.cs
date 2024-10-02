using Application.BaseManager;
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
    public class StudentDashobardController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        private readonly CoursesService _coursesService;
        private readonly DepartmentService _departmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StudentDashobardController(APIResponse aPIResponse, ILogger<AssignmentController> _logger, IMapper mapper, IAssignmentEnrollService assignmentEnrollService, CoursesService coursesService, DepartmentService departmentService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentEnrollService = assignmentEnrollService;
            _coursesService = coursesService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("assignment/recent")]
        public async Task<IActionResult> GetAlltudentRecentAssignment()
        {
            var data = await _assignmentEnrollService.GetAllStudentAssignment(1, 5, "","", CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetStudentAssignmentSummary()
        {
            var data = await _assignmentEnrollService.GetStudentAssignementSummary(CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }
    }
}
