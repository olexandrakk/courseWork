using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile() 
        {
            CreateMap<CreateReviewRequest, Review>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Review, ReviewDto>();
        }
    }
}
