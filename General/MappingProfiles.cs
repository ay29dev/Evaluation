using AutoMapper;
using Evaluation.Dtos;
using Evaluation.Models;

namespace Evaluation.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Employee, EmployeeDto>();
        }
    }
}