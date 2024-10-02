using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class AssignmentContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public AssignmentContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
