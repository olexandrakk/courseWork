using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests.Inventory;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Profiles
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<CreateInventoryRequest, Inventory>();

            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Name))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.BookStore.Name));
        }
    }
}