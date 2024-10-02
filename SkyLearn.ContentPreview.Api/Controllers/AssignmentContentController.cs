using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.ContentPreview.Api.Services;

namespace SkyLearn.ContentPreview.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentContentService _assignmentContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public AssignmentContentController(APIResponse aPIResponse, ILogger<AssignmentContentController> _logger, IMapper mapper, AssignmentContentService assignmentContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger ,contextAccessor)
        {
            _mapper = mapper;
            _assignmentContentService = assignmentContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<AssignmentDTO>>> GetAllAssignment()
        {
            try
            {
                var data = await _assignmentContentService.List<Assignment,AssignmentDTO>();
                var content = _mapper.Map<List<AssignmentDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignment list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]

        public async Task<ActionResult<AssignmentDTO>> GetDetails(string Pid)
        {
            try
            {
                Assignment data = await _assignmentContentService.Retrieve<Assignment>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<AssignmentDTO>(data);
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
