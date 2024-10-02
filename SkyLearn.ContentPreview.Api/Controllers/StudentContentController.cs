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
    public class StudentContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly StudentContentService _studentContentService;
        private readonly IHttpContextAccessor _contextAccessor;

        public StudentContentController(APIResponse aPIResponse, ILogger<StudentContentController> _logger, IMapper mapper, StudentContentService studentContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _studentContentService = studentContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<StudentDTO>>> GetAllStudents()
        {
            try
            {
                var data = await _studentContentService.List<Application.Models.Student,StudentDTO>();
                var content = _mapper.Map<List<StudentDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Student list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]
        public async Task<ActionResult<StudentDTO>> GetDetails(string Pid)
        {
            try
            {
                Application.Models.Student data = await _studentContentService.Retrieve<Application.Models.Student>(Pid);
                if (data != null)
                {
                    var res = _mapper.Map<StudentDTO>(data);
                    return this.OnSuccess(res, 200);
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
