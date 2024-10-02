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
    public class ParentAssignmentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentEnrollService _assignmentEnrollService;
        public ParentAssignmentController(APIResponse aPIResponse, ILogger<StudentController> _logger, IMapper mapper, IAssignmentEnrollService assignmentEnrollService, IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _assignmentEnrollService = assignmentEnrollService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllParentStudentAssignment(string? searchText, string? status, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetAllParentStudentAssignment(pageNumber, pageSize, searchText, status, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParentStudentAssignementDetail(string id)
        {
            var data = await _assignmentEnrollService.GetParentStudentAssignementDetail(id, CurrentUserID);
            return this.OnSuccess(data, (int)HttpStatusCode.OK);
        }

        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetParentSTudentAssignementLogList(string id, bool paginate = false, int pageSize = 10, int pageNumber = 1)
        {
            var data = await _assignmentEnrollService.GetParentSTudentAssignementLogList(pageNumber, pageSize, id, CurrentUserID);
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
