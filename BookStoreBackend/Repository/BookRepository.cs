using BookStoreBackend.Data;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using AutoMapper;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BookModel> GetBookById(string id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<BookModel>> GetAllBooks()
    {
        var books = await _context.Books
                             .AsNoTracking()
                             .ToListAsync();
        if (books != null && books.Any())
        {
            return books;
        }
        return Enumerable.Empty<BookModel>();
    }
    public async Task<int> GetBookCount()
    {
        return await _context.Books.CountAsync();
    }

    public async Task RegisterBook(BookViewModel bookDto)
    {
        var newBook = _mapper.Map<BookModel>(bookDto);
        await _context.Books.AddAsync(newBook);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBook(string id, BookViewModel updatedDto)
    {
        var book = await GetBookById(id);
        if (book != null)
        {
            _mapper.Map(updatedDto, book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteBook(string id)
    {
        var book = await GetBookById(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
