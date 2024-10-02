using Application;
using Application.BaseManager;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api.Services
{
    public class AssignmentService : RestService
    {
        private readonly Context _context;
        public AssignmentService(Context context, IMapper mapper) : base(context, mapper)
        { 
            _context = context;
        }

        public override Task<T> Retrieve<T>(string Pid)
        {
            _context.Assignments
                .Include(dep => dep.Department).Include(cou => cou.Course).Include(sem =>sem.Semester).ToList();

            return base.Retrieve<T>(Pid);
        }
    }
}
