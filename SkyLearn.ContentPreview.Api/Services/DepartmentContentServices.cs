using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.Services
{
    public class DepartmentContentServices : RestService
    {
        private readonly ContentContextPreview _context;
        public DepartmentContentServices(ContentContextPreview context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
