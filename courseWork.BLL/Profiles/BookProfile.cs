using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.DAL.Entities;

namespace courseWork.BLL.Common.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<CreateBookRequest, Book>()
                .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher.Name))
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.BookAuthors.Select(ba => ba.Author)));
        }
    }
}