using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.ContentPreview.Api
{
    public class ContentContextPreview : DbContext
    {
        private readonly IConfiguration _configuration;

        public ContentContextPreview(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this._configuration.GetConnectionString("LocalDB"), b => b.MigrationsAssembly("SkyLearn.Portal.Api"));
            }
        }

        public DbSet<Content> Contents { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Downloads> Downloads { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormData> FormDatas { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<AssignmentData> AssignmentDatas { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentData> ComponentDatas { get; set; }

    }
}
