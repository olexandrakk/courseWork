using AutoMapper;
using courseWork.BLL.Profiles;

namespace courseWork.Tests.Helpers
{
    public static class TestHelper
    {
        public static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AuthorProfile>();
                cfg.AddProfile<BookProfile>();
                cfg.AddProfile<BookStoreProfile>();
                cfg.AddProfile<ReviewProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<PublisherProfile>();
                cfg.AddProfile<InventoryProfile>();
                cfg.AddProfile<OrderProfile>();
            });

            return configuration.CreateMapper();
        }
    }
}

