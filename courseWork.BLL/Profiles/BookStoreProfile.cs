using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Profiles
{
    public class BookStoreProfile : Profile
    {
        public BookStoreProfile()
        {
            CreateMap<CreateBookStoreRequest, BookStore>();

            CreateMap<BookStore, BookStoreDto>();
        }
    }
}