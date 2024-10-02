using Application.Models;
using AutoMapper;

namespace SkyLearn.ContentPreview.Api.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //content mapping
            CreateMap<Content, ContentDTO>().ReverseMap();
            //department mapping
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            //semester mapping
            CreateMap<Semester, SemesterDTO>().ReverseMap();
            //Assignment mapping
            CreateMap<Assignment, AssignmentDTO>().ReverseMap();
            //AssignmentData mapping
            CreateMap<AssignmentData, AssignmentDataDTO>().ReverseMap();

            //Form Mapping
            CreateMap<Form, FormDTO>().ReverseMap();

            //FormData Mappin
            CreateMap<FormData, FormDataDTO>().ReverseMap();
            CreateMap<FormData, CreateFormDataDTO>().ReverseMap();

            //Result mapping
            CreateMap<Result, ResultDTO>().ReverseMap();

            //Staff mapping
            CreateMap<Staff, StaffDTO>().ReverseMap();
            //CreateMap<Staff, CreateStaffDTO>().ReverseMap();

            //Student mapping
            CreateMap<Student, StudentDTO>().ReverseMap();
//CreateMap<Student, CreateStudentDTO>().ReverseMap();


        }
    }
}
