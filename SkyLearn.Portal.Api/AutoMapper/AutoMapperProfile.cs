using Application.Models;
using AutoMapper;
using static Application.Models.Roles;

namespace SkyLearn.Portal.Api.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //content mapping
            CreateMap<Content, ContentDTO>().ReverseMap();
            CreateMap<Content, CreateContentDTO>().ReverseMap();
            //department mapping
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<Department, CreateDepartmentDTO>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDTO>().ReverseMap();
            //semester mapping
            CreateMap<Semester, SemesterDTO>().ReverseMap();
            CreateMap<Semester,SemesterListDTO>().ReverseMap();
            CreateMap<Semester, CreateSemesterDTO>().ReverseMap();
            CreateMap<Semester, UpdateSemesterDTO>().ReverseMap();
            //Assignment mapping
            CreateMap<Assignment, AssignmentDTO>().ReverseMap();
            CreateMap<Assignment, CreateAssignmentDTO>().ReverseMap();
            CreateMap<Assignment, AssignmentDetailDTO>()
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => new DepartmentsDTO { Pid = src.Department.Pid, Name = src.Department.DepartmentName }))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => new CoursesDTO { Pid = src.Course.Pid, Name = src.Course.Title }))
                .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => new SemestersDTO { Pid = src.Semester.Pid, Name = src.Semester.Name }))
                //.ForMember(dest => dest.Staff, opt => opt.MapFrom(src => new StaffsDTO { Pid = src.Staff.Pid, Name = src.Staff.Name,Email = src.Staff.Email }))
                .ReverseMap();
            //AssignmentData mapping
            CreateMap<AssignmentData, AssignmentDataDTO>().ReverseMap();

            //Form Mapping
            CreateMap<Form, FormDTO>().ReverseMap();

            //FormData Mappin
            CreateMap<FormData, FormDataDTO>().ReverseMap();

            //Result mapping
            CreateMap<Result, ResultDTO>().ReverseMap();

            //Roles mapping
            CreateMap<Roles, RolesDTO>().ReverseMap();
            CreateMap<Roles, CreateRoleDTO>().ReverseMap();
            CreateMap<Roles, UpdateRoleDTO>().ReverseMap();
            //Staff mapping
  

            //Student mapping

            //component mapping
            CreateMap<Component, ComponentDTO>().ReverseMap();
            //Courses mapping
            CreateMap<Courses, CourseDTO>().ReverseMap();
            CreateMap<Courses, CreateCoursesDTO>().ReverseMap();
            CreateMap<Courses,CourseDetailDTO>().ReverseMap();
            CreateMap<Courses,UpdateCoursesDTO>().ReverseMap();

            CreateMap<Users, UserDTO>().ReverseMap();
            CreateMap<Users, CreateUserDTO>().ReverseMap();
            CreateMap<IdentityUserDTO, CreateUserDTO>().ReverseMap();
            //userrole mapping
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<UserRole, CreateUserRoleDTO>().ReverseMap();
            CreateMap<UserProfile, Student>().ReverseMap();
            CreateMap<UserProfile, Staff>().ReverseMap();
            CreateMap<Student, CreateUserDTO>().ReverseMap();
            CreateMap<Student, Staff>().ReverseMap();
    
        }
    }
}
