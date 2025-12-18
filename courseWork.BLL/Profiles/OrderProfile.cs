using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Common.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<OrderDetails, OrderDetailDto>()
                .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => src.Book.Name));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.UpdateAt ?? DateTime.UtcNow))

                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));
        }
    }
}