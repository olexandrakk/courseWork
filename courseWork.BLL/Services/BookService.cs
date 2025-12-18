using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<Author> _authorRepository;
        private readonly IMapper _mapper;

        public BookService(
            IRepository<Book> bookRepository,
            IMapper mapper,
            IRepository<Author> authorRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookRequest request)
        {
            var existingBook = await _bookRepository
                .FirstOrDefaultAsync(x => x.ISBN == request.ISBN);

            if (existingBook != null)
                throw new InvalidOperationException(
                    $"Book with ISBN {request.ISBN} already exists.");

            var book = _mapper.Map<Book>(request);

            var authors = await _authorRepository
                .Where(a => request.AuthorIds.Contains(a.AuthorID))
                .ToListAsync();

            if (authors.Count != request.AuthorIds.Count)
                throw new InvalidOperationException("One or more authors not found.");

            foreach (var author in authors)
            {
                book.Authors.Add(author);
            }

            await _bookRepository.InsertAsync(book);

            return await GetBookByIdAsync(book.BookID);
        }


        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .ToListAsync();

            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository
                .AsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.BookID == id);

            return _mapper.Map<BookDto>(book);
        }
    }
}