using Application.Response;
using Microsoft.Extensions.DependencyInjection;
using SkyLearn.Portal.Api.Controllers;
using SkyLearn.Portal.Api.Interfaces;
using SkyLearn.Portal.Api.Service;
using SkyLearn.Portal.Api.Services;

namespace SkyLearn.Portal.Api.DependencyInjection
{
    public class DependencyInjectionConfig
    {
        public static void Configure(IServiceCollection services)
        {
            //injecting other services here
            services.AddScoped<APIResponse>();

            //injecting model serivces here 
            services.AddScoped<ContentService>();
            services.AddScoped<DepartmentService>();
            services.AddScoped<SemesterService>();
            services.AddScoped<AssignmentService>();
            services.AddScoped<AssignmentDataService>();
            services.AddScoped<CoursesService>();
            services.AddScoped<ResultService>();
            services.AddScoped<RolesService>();
            services.AddScoped<FormService>();
            services.AddScoped<FormDataService>();
            services.AddScoped<PageActionService>();
            services.AddScoped<UserRoleService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<ComponentService>();
            services.AddScoped<UsersService>();
            services.AddScoped<UserController>();
            services.AddScoped<ActionPermissionService>();
            services.AddScoped<MenuService>();
            services.AddScoped<StatusService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IStaffDapperService, StaffDapperService>();
            services.AddSingleton<IStudentDapperService, StudentDapperService>();
            services.AddSingleton<IAssignmentEnrollService, AssignmentEnrollService>();
            services.AddSingleton<IEmailSendService, EmailSendService>();
            
        }
    }
}
