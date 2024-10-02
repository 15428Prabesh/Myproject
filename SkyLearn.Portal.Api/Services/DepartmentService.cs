using Application;
using Application.BaseManager;
using AutoMapper;


namespace SkyLearn.Portal.Api.Services
{
    public class DepartmentService : RestService
    {
        private readonly Context _context;
        public DepartmentService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

    }
}
