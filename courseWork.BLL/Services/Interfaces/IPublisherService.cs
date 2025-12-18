using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;

namespace courseWork.BLL.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherRequest request);
        Task<List<PublisherDto>> GetAllPublishersAsync();
        Task<PublisherDto> UpdatePublisherAsync(int id, CreatePublisherRequest request);
        Task DeletePublisherAsync(int id);
    }
}
