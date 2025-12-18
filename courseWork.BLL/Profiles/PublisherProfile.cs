using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Profiles
{
    public class PublisherProfile : Profile
    {
        public PublisherProfile()
        {
            CreateMap<CreatePublisherRequest, Publisher>();

            CreateMap<Publisher, PublisherDto>();
        }
    }
}
