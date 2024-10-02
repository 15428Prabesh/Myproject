using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class ResultService : RestService
    {
        private readonly Context _context;
        public ResultService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
