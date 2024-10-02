using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class AssignmentDataContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public AssignmentDataContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
