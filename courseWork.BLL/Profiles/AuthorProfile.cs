using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<CreateAuthorRequest, Author>();

            CreateMap<Author, AuthorDto>();
        }
    }
}
