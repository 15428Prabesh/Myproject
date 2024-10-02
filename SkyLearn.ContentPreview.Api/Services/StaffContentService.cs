using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class StaffContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public StaffContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
