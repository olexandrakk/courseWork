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

        public async Task<PublisherDto> UpdatePublisherAsync(int id, CreatePublisherRequest request)
        {
            var publisher = await _publisherRepository
                .FirstOrDefaultAsync(p => p.PublisherID == id);

            if (publisher == null)
                throw new Exception($"Publisher with current Id doesn't exist: {id}");

            var nameExists = await _publisherRepository
                .FirstOrDefaultAsync(p => p.Name == request.Name && p.PublisherID != id);

            if (nameExists != null)
                throw new InvalidOperationException("Publisher with this name already exists.");

            publisher.Name = request.Name;
            publisher.Address = request.Address;

            await _publisherRepository.UpdateAsync(publisher);

            return _mapper.Map<PublisherDto>(publisher);
        }

        public async Task DeletePublisherAsync(int id)
        {
            var publisher = await _publisherRepository
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.PublisherID == id);

            if (publisher == null)
                throw new Exception($"Publisher with current Id doesn't exist: {id}");

            if (publisher.Books.Any())
                throw new InvalidOperationException("Cannot delete publisher with associated books.");

            await _publisherRepository.DeleteAsync(publisher);
        }
    }
}
