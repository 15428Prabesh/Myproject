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
    public class DepartmentContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly DepartmentContentServices _departmentcontentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public DepartmentContentController(APIResponse aPIResponse, ILogger<DepartmentContentController> _logger, IMapper mapper, DepartmentContentServices departmentcontentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _departmentcontentService = departmentcontentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDTO>>> GetAllDepartments()
        {
            try
            {
                var data = await _departmentcontentService.List<Department,DepartmentDTO>();
                var content = _mapper.Map<List<DepartmentDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching department list {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("{Pid}")]

        public async Task<ActionResult<DepartmentDTO>> GetDetails(string Pid)
        {
            try
            {
                Department data = await _departmentcontentService.Retrieve<Department>(Pid);
                if (data != null)
                {
                    var dto_data = _mapper.Map<DepartmentDTO>(data);
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
