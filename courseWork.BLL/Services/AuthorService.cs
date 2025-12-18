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

        public async Task<AuthorDto> UpdateAuthorAsync(int authorId, CreateAuthorRequest request)
        {
            var author = await _authorRepository
                .FirstOrDefaultAsync(a => a.AuthorID == authorId);

            if (author == null)
                throw new KeyNotFoundException("Author not found.");

            var nameExists = await _authorRepository
                .FirstOrDefaultAsync(a => a.Name == request.Name && a.AuthorID != authorId);

            if (nameExists != null)
                throw new InvalidOperationException("Author with the same name already exists.");

            author.Name = request.Name;

            await _authorRepository.UpdateAsync(author);

            return _mapper.Map<AuthorDto>(author);
        }


        public async Task DeleteAuthorAsync(int authorId)
        {
            var author = await _authorRepository
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.AuthorID == authorId);

            if (author == null)
                throw new KeyNotFoundException("Author not found.");

            foreach (var book in author.Books.ToList())
            {
                author.Books.Remove(book);
            }

            await _authorRepository.DeleteAsync(author);
        }
    }
}
