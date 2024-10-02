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
    public class AdminStudentAssignmentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentService _assignmentService;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        private readonly CoursesService _coursesService;
        private readonly DepartmentService _departmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminStudentAssignmentController(APIResponse aPIResponse, ILogger<AssignmentController> _logger, IMapper mapper, AssignmentService assignmentService, CoursesService coursesService, DepartmentService departmentService, IAssignmentEnrollService assignmentEnrollService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentService = assignmentService;
            _coursesService = coursesService;
            _departmentService = departmentService;
            _httpContextAccessor = httpContextAccessor;
            _assignmentEnrollService = assignmentEnrollService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdminStudentAssignment(string? searchText, string? status, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetAllAdminStudentAssignment(pageSize, pageNumber, searchText, status, CurrentUserID);
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