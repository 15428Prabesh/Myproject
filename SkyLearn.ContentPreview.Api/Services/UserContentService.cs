using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class UserContentService : RestService
    {
        private readonly ContentContextPreview _context;
        public UserContentService(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
