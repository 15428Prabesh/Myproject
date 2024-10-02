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
    public class StaffContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly StaffContentService _staffContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public StaffContentController(APIResponse aPIResponse, ILogger<StaffContentController> _logger, IMapper mapper, StaffContentService staffContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _staffContentService = staffContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<StaffDTO>>> GetAllStaffs()
        {
            try
            {
                var data = await _staffContentService.List<Application.Models.Staff,StaffDTO>();
                var content = _mapper.Map<List<StaffDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Staff list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<StaffDTO>> GetDetails(string Pid)
        {
            try
            {
                Application.Models.Staff data = await _staffContentService.Retrieve<Application.Models.Staff>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<StaffDTO>(data);
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
