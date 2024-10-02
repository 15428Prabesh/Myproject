using Application.BaseManager;
using AutoMapper;
using SkyLearn.Portal.Api;

namespace SkyLearn.Portal.Api.Services
{
    public class AssignmentDataService : RestService
    {
        private readonly Context _context;
        public AssignmentDataService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
    
}
