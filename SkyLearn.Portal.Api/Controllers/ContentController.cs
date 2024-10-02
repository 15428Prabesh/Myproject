using Application.Models;
using Application.Response;
using Microsoft.AspNetCore.Mvc;
using Application.BaseManager;
using SkyLearn.Portal.Api.Service;
using Application.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Cors;
using SkyLearn.Portal.Api.Middleware;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace SkyLearn.Portal.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(AuthorizedUser))]
    public class ContentController : BaseAPIController
    {
        private readonly ContentService contentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public ContentController(APIResponse aPIResponse, ILogger<ContentController> _logger, ContentService restService, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(aPIResponse, _logger , httpContextAccessor)
        {
            contentService = restService;
            _mapper = mapper;
            _httpContextAccessor=httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult<List<ContentDTO>>> GetAllContent(bool paginate=false,int pageSize=10,int pageNumber=1)
        {
            try
            {

                var data = await contentService.List<Content,ContentDTO>(paginate,pageSize,pageNumber);
                return this.OnSuccess(data, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while fetching content data {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContentDTO>> CreateContent(CreateContentDTO contentView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (contentService.ContentExists(contentView.Title))
                    {
                        ModelState.AddModelError("Title", "A content with the same title already exists.");
                        return this.OnBadRequest("A content with the same title already exists.", "validation", (int)HttpStatusCode.BadRequest);
                    }
                    var contentData = _mapper.Map<Content>(contentView);
                    contentData.Pid = AppHelper.GeneratePid(Constant.PREFIX_CONTENT);
                    contentData.CreatedAt = DateTime.UtcNow;
                    contentData.CreatedBy = CurrentUserName;
                    var data = _mapper.Map<ContentDTO>(await contentService.Create(contentData));
                    return this.OnSuccess(data, (int)HttpStatusCode.OK);
                }
                return this.OnBadRequest("Fill all the required forms", "validation");
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while creating content.{0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{Pid}")]
        public async Task<ActionResult<ContentDTO>> GetDetail(string Pid)
        {
            try
            {

                var res = await contentService.Retrieve<Content>(Pid);

                if (res == null)
                {
                    return this.OnBadRequest("Invalid content pid", "validation", 400);
                }
                var data = _mapper.Map<ContentDTO>(res);
                return this.OnSuccess(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Obtaining content details {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPut("{Pid}")]
        public async Task<ActionResult<ContentDTO>> UpdateContent(string Pid,CreateContentDTO fields)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await contentService.Retrieve<Content>(Pid);
                    if (data == null)
                    {
                        return this.OnNotFound("Invalid Content ID", "not_found");
                    }

                    data.UpdatedAt = DateTime.UtcNow;
                    data.UpdatedBy = "superuser";
                    data.Details = fields.Details;
                    data.IsTitleVisible = fields.IsTitleVisible;
                    data.IsSummaryVisible = fields.IsSummaryVisible;
                    data.Summary = fields.Summary;
                    data.Title = fields.Title;
                    data.IsModified = true;
                    var result = await contentService.UpdatePut(Pid, data);
                    var detail=_mapper.Map<ContentDTO>(result);
                    return this.OnSuccess(detail, (int)HttpStatusCode.OK);

                }
                return this.OnBadRequest("Enter all the required fields","validation", (int)HttpStatusCode.BadRequest);
            }
            catch(Exception ex)
            {
                this._logger.LogError("Error while updating content {0}", ex);
                throw ex;

            }
     
        }


        [HttpDelete("{Pid}")]
        public async Task<ActionResult<string>> DeleteContent(string Pid)
        {
            try
            {
                var instance = await contentService.Retrieve<Content>(Pid);
                if (instance == null)
                {
                    return this.OnNotFound("Content Not Found", "not found", (int)HttpStatusCode.NotFound);
                }
                var res = await contentService.Delete(instance, false);
                return this.OnSuccess("Content Deleted successfully", (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                this._logger.LogError("Error while Deleting content {0}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}

