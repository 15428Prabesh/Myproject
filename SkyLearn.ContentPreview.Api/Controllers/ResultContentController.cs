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
    public class ResultContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly ResultContentService _resultContentService;
        private readonly IHttpContextAccessor _contextAccessor;
        public ResultContentController(APIResponse aPIResponse, ILogger<ResultContentController> _logger, IMapper mapper, ResultContentService resultContentService, IHttpContextAccessor contextAccessor) : base(aPIResponse, _logger, contextAccessor)
        {
            _mapper = mapper;
            _resultContentService = resultContentService;
            _contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<ResultDTO>>> GetAllResult()
        {
            try
            {
                var data = await _resultContentService.List<Result,ResultDTO>();
                var content = _mapper.Map<List<ResultDTO>>(data);
                return this.OnSuccess(content, 200);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Result {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<ResultDTO>> GetDetail(string Pid)
        {
            try
            {
                var entity = await _resultContentService.Retrieve<Result>(Pid);
                if (entity == null)
                {
                    return this.OnBadRequest("Invalid Result", "validation", 400);
                }
                var data = _mapper.Map<ResultDTO>(entity);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining Result details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }        
    }
}
