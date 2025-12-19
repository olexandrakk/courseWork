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

            CreateMap<UpdateStockRequest, Inventory>()
                .ForMember(dest => dest.InventoryID, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.BookStore, opt => opt.Ignore())
                .ForMember(dest => dest.BookID, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BookStoreID, opt => opt.MapFrom(src => src.BookStoreId))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.NewQuantity));

            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Name))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.BookStore.Name));
        }
    }
}