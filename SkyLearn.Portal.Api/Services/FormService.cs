using Application;
using Application.BaseManager;
using AutoMapper;

namespace SkyLearn.Portal.Api.Services
{
    public class FormService : RestService
    {
        private readonly Context _context;
        public FormService(Context context, IMapper mapper) : base(context, mapper)
        { 
            _context = context;
        }
    }
}
