using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IRepository<Publisher> _publisherRepository;
        private readonly IMapper _mapper;

        public PublisherService(IRepository<Publisher> publisherRepository, IMapper mapper)
        {
            _publisherRepository = publisherRepository;
            _mapper = mapper;
        }

        public async Task<List<PublisherDto>> GetAllPublishersAsync()
        {
            var publishers = await _publisherRepository.ToListAsync();
            return _mapper.Map<List<PublisherDto>>(publishers);
        }

        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherRequest request)
        {
            var existing = await _publisherRepository.FirstOrDefaultAsync(x => x.Name == request.Name);
            if (existing != null)
            {
                throw new InvalidOperationException("Publisher with this name already exists.");
            }

            var publisher = _mapper.Map<Publisher>(request);

            await _publisherRepository.InsertAsync(publisher);

            return _mapper.Map<PublisherDto>(publisher);
        }
    }
}
