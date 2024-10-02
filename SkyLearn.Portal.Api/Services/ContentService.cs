using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api.Service
{
    public class ContentService : RestService
    {
        private readonly Context _context;

        public ContentService(Context context, IMapper mapper) : base(context, mapper)
        {
            _context = context;

        }
        public bool ContentExists(string title)
        {
            return _context.Contents.Any(x => x.Title == title);
        }

    }
}
