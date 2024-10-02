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
    public class AssignmentDataContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly AssignmentDataContentService _assignmentDataContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public AssignmentDataContentController(APIResponse aPIResponse, ILogger<AssignmentDataContentController> _logger, IMapper mapper, AssignmentDataContentService assignmentdataContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _assignmentDataContentService = assignmentdataContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<AssignmentDataDTO>>> GetAllAssignmentData()
        {
            try
            {
                var data = await _assignmentDataContentService.List<AssignmentData,AssignmentDataDTO>();
                var content = _mapper.Map<List<AssignmentDataDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Assignment Data list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]

        public async Task<ActionResult<AssignmentDataDTO>> GetDetails(string Pid)
        {
            try
            {
                AssignmentData data = await _assignmentDataContentService.Retrieve<AssignmentData>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<AssignmentDataDTO>(data);
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
