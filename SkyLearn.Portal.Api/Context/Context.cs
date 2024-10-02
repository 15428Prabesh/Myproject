using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace SkyLearn.Portal.Api
{
    public class Context : DbContext
    {
        private readonly IConfiguration _configuration;

        public Context(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this._configuration.GetConnectionString("LocalDB"));
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration for the Department relationship
            modelBuilder.Entity<Courses>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the Semester relationship
            modelBuilder.Entity<Semester>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the Staff relationship
            //modelBuilder.Entity<Staff>()
            //    .HasOne(c => c.Department)
            //    .WithMany()
            //    .HasForeignKey(c => c.DepartmentId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the Result relationship
            modelBuilder.Entity<Result>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the Courses relationship
            modelBuilder.Entity<Courses>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Courses>()
                .HasOne(c => c.Semester)
                .WithMany()
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the Assignment relationship
            modelBuilder.Entity<Assignment>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(c => c.Semester)
                .WithMany()
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Assignment>()
            //    .HasOne(c => c.Staff)
            //    .WithMany()
            //    .HasForeignKey(c => c.StaffId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //Configuration for the AssignmentData relationship

            modelBuilder.Entity<AssignmentData>()
                 .HasOne(c => c.Assignment)
                 .WithMany()
                 .HasForeignKey(c => c.AssignmentId)
                 .OnDelete(DeleteBehavior.Restrict);

            // Configuration for the FormData relationship
            modelBuilder.Entity<FormData>()
                .HasOne(c => c.Form)
                .WithMany()
                .HasForeignKey(c => c.FormID)
                .OnDelete(DeleteBehavior.Restrict);

            // Other entity configurations...

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Content> Contents { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Downloads> Downloads { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormData> FormDatas { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<AssignmentData> AssignmentDatas { get; set; }
        public DbSet<ControllerAction> ControllerActions { get; set; }
        public DbSet<ActionPermission> ActionPermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentData> ComponentDatas { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Status> Status { get; set; }


    }
}
