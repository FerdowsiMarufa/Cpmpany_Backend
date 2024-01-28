using AutoMapper;
using Company.Model;
using Company.Model.Dto;

namespace Company
{
    public class MappingConfig :Profile
    {
        public MappingConfig() { 
          CreateMap<User , UserDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<UserDto, LoginDto>().ReverseMap();
            CreateMap<EmployeeModel, EmployeeWithCertificateDto>().ReverseMap();
        }

    }
}

