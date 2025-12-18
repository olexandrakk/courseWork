using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.DBContext;
using courseWork.DAL.Entities;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<BookAuthor> _bookAuthorRepository; 
        private readonly IMapper _mapper;

        public BookService(
            IRepository<Book> bookRepository,
            IRepository<BookAuthor> bookAuthorRepository, 
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _bookAuthorRepository = bookAuthorRepository;
            _mapper = mapper;
        }

        public async Task<BookDto> CreateBookAsync(CreateBookRequest request)
        {
            var existingBook = await _bookRepository.FirstOrDefaultAsync(x => x.ISBN == request.ISBN);
            if (existingBook != null)
            {
                throw new InvalidOperationException($"Book with ISBN {request.ISBN} already exists.");
            }

            var book = _mapper.Map<Book>(request);

            await _bookRepository.InsertAsync(book);

            foreach (var authorId in request.AuthorIds)
            {
                var bookAuthor = new BookAuthor
                {
                    BookID = book.BookID,   
                    AuthorID = authorId    
                };

                await _bookAuthorRepository.InsertAsync(bookAuthor);
            }

            return await GetBookByIdAsync(book.BookID);
        }

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            var books = await _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .ToListAsync();

            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository
                .AsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.BookID == id);

            return _mapper.Map<BookDto>(book);
        }
    }
}