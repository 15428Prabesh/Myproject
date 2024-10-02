using Application.BaseManager;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http.Connections;
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
    public class StaffDashboardController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentService _assignmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        public StaffDashboardController(APIResponse aPIResponse, ILogger<AssignmentController> _logger, IMapper mapper, AssignmentService assignmentService, CoursesService coursesService, DepartmentService departmentService, IAssignmentEnrollService assignmentEnrollService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentService = assignmentService;
            _httpContextAccessor = httpContextAccessor;
            _assignmentEnrollService = assignmentEnrollService;
        }

        [HttpGet("assignment/recent")]
        public async Task<IActionResult> GetAllRecentAssignment()
        {
            var data = await _assignmentEnrollService.GetAllStaffAssignment(5,1,"", "", CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAssignmentSummary()
        {
            var data = await _assignmentEnrollService.GetStaffStudentAssignementSummary(CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }
    }
}
