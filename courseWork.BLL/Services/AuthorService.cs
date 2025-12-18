using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IRepository<Author> _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IRepository<Author> authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorRequest request)
        {
            var existingAuthor = await _authorRepository.FirstOrDefaultAsync(x => x.Name == request.Name);

            if (existingAuthor != null)
            {
                throw new InvalidOperationException("Author with the same name already exists.");
            }

            var author = _mapper.Map<Author>(request);

            await _authorRepository.InsertAsync(author);

            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<List<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.ToListAsync();

            return _mapper.Map<List<AuthorDto>>(authors);
        }
    }
}
