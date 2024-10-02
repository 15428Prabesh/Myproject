using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.ContentPreview.Api.Services;

namespace SkyLearn.ContentPreview.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly CoursesContentService _coursesContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public CoursesContentController(APIResponse aPIResponse, ILogger<CoursesContentController> _logger, IMapper mapper, CoursesContentService coursesContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger,contextAccessor)
        {
            _mapper = mapper;
            _coursesContentService = coursesContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<CourseDTO>>> GetAllAssignment()
        {
            try
            {
                var data = await _coursesContentService.List<Courses,CourseDTO>();
                var content = _mapper.Map<List<CourseDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Courses list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]

        public async Task<ActionResult<CourseDTO>> GetDetails(string Pid)
        {
            try
            {
                Courses data = await _coursesContentService.Retrieve<Courses>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<CourseDTO>(data);
                    return this.OnSuccess(dto_data, 200);
                }
                return this.OnNotFound("Data not found", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while retrieving data . {0}", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
