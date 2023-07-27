using AutoMapper;
using MyBackend_MongoDB_CSharp.Models;
using MyBackend_MongoDB_CSharp.Models.DTOs;

namespace MyBackend_MongoDB_CSharp.Helpers
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            CreateMap<AuthDTO, Auth>();
        }
    }
}
