using Application.Response;
using SkyLearn.ContentPreview.Api.Services;

namespace SkyLearn.ContentPreview.Api.DependencyInjections
{
    public class DependencyInjections
    {
        public static void Configure(IServiceCollection services)
        {
            //injecting other services here
            services.AddScoped<APIResponse>();

            //injecting model serivces here 
            services.AddScoped<DepartmentContentServices>();
            services.AddScoped<AssignmentContentService>();
            services.AddScoped<ContentService>();
            services.AddScoped<SemesterContentService>();
            services.AddScoped<AssignmentDataContentService>();
            services.AddScoped<CoursesContentService>();
            services.AddScoped<ResultContentService>();
            services.AddScoped<StudentContentService>();
            services.AddScoped<StaffContentService>();
            services.AddScoped<FormContentService>();
            services.AddScoped<FormDataContentService>();
        }
    }
}
