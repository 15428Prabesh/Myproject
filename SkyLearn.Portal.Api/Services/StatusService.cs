using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class StatusService : RestService
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        public StatusService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
