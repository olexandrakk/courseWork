using AutoMapper;
using courseWork.BLL.Common.DTO;
using courseWork.BLL.Common.Requests;
using courseWork.BLL.Common.Requests.Pagination;
using courseWork.BLL.Extensions;
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


        public async Task<PagedResult<BookDto>> GetAllBooksAsync(GetBooksRequest request)
        {
            var query = _bookRepository
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(b =>
                    b.Name.Contains(request.Name));
            }

            if (request.AuthorId.HasValue)
            {
                query = query.Where(b =>
                    b.Authors.Any(a => a.AuthorID == request.AuthorId));
            }

            var pagedBooks = await query
                .ToPagedResultAsync(request.Page, request.PageSize);

            return new PagedResult<BookDto>
            {
                Items = _mapper.Map<List<BookDto>>(pagedBooks.Items),
                Page = pagedBooks.Page,
                PageSize = pagedBooks.PageSize,
                TotalCount = pagedBooks.TotalCount
            };
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _bookRepository
                .AsNoTracking()
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.BookID == id);

            if (book == null)
                throw new Exception($"Book with current Id doesn't exist: {id}");

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> UpdateBookAsync(int id, CreateBookRequest request)
        {
            var book = await _bookRepository
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.BookID == id);

            if (book == null)
                throw new Exception($"Book with current Id doesn't exist: {id}");

            if (book.ISBN != request.ISBN)
            {
                var existingBook = await _bookRepository
                    .FirstOrDefaultAsync(x => x.ISBN == request.ISBN);

                if (existingBook != null)
                    throw new InvalidOperationException(
                        $"Book with ISBN {request.ISBN} already exists.");
            }

            book.ISBN = request.ISBN;
            book.Name = request.Name;
            book.NumberOfPages = request.NumberOfPages;
            book.Price = request.Price;
            book.PublisherID = request.PublisherID;

            var authors = await _authorRepository
                .Where(a => request.AuthorIds.Contains(a.AuthorID))
                .ToListAsync();

            if (authors.Count != request.AuthorIds.Count)
                throw new InvalidOperationException("One or more authors not found.");

            book.Authors.Clear();
            foreach (var author in authors)
            {
                book.Authors.Add(author);
            }

            await _bookRepository.UpdateAsync(book);

            return await GetBookByIdAsync(book.BookID);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _bookRepository
                .FirstOrDefaultAsync(b => b.BookID == id);

            if (book == null)
                throw new Exception($"Book with current Id doesn't exist: {id}");

            book.IsDeleted = true;
            book.DeletedAt = DateTime.UtcNow;

            await _bookRepository.UpdateAsync(book);
        }
    }
}