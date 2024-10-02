using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api.Services
{
    public class CoursesService : RestService
    {
        private readonly Context _context;
        public CoursesService(Context context,IMapper mapper) : base(context,mapper)
        {
            _context = context;
        }

        public override Task<T> Retrieve<T>(string Pid)
        {
            _context.Courses
                .Include(dep => dep.Department).Include(sem => sem.Semester).ToList();

            return base.Retrieve<T>(Pid);
        }
    }
}
