using Application.BaseManager;
using Application.Models;
using Application.Response;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkyLearn.ContentPreview.Api.Services;
using System.Security.AccessControl;

namespace SkyLearn.ContentPreview.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : BaseAPIController
    {
        private readonly IMapper _mapper;
        private readonly ContentService _contentService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ContentController(APIResponse aPIResponse, ILogger<ContentController> _logger, IMapper mapper, ContentService contentService,IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger, httpContextAccessor)
        {
            _mapper = mapper;
            _contentService = contentService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<ContentDTO>> GetContent(string Pid)
        {
            try
            {
                Content data = await _contentService.Retrieve<Content>(Pid);
                if (data != null)
                {
                    return this.OnSuccess(data, 200);
                }
                return this.OnNotFound("No content found with the provided title.", "error", 404);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching Content by title: {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}


