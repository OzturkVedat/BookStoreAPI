using BookStoreBackend.Data;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BookStoreBackend.Models.ResultModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultModel> GetBookById(string id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (book == null)
        {
            return new ErrorResult($"No book found with ID: {id}");
        }
        return new SuccessDataResult<BookModel>("Book retrieved successfully.", book);
    }

    public async Task<ResultModel> GetBooksByTitle(string title)
    {
        var books = await _context.Books.Where(b => b.Title == title).ToListAsync();
        if (books.Any())
            return new SuccessDataResult<IEnumerable<BookModel>>("Books retrieved successfully.", books);
        return new ErrorResult($"No book found with title: {title}");
    }
    public async Task<bool> BookWithSameIsbnExists(string isbn)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
        return book != null;
    }

    public async Task<ResultModel> GetAllBooks(int page, int pageSize)
    {
        var books= await _context.Books
                             .AsNoTracking()
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
        return (books.Any())
        ? new SuccessDataResult<IEnumerable<BookModel>>("Successfully fetched the requested books.", books)
        : new ErrorResult("No books found for the requested page.");
    }

    public async Task<int> GetBookCount()
    {
        return await _context.Books.CountAsync();
    }

    public async Task<ResultModel> RegisterBook(BookViewModel bookDto)
    {
        if (bookDto.ISBN != null)
        {
            var bookExists = await BookWithSameIsbnExists(bookDto.ISBN);
            if (bookExists)
                return new ErrorResult("Book with the same ISBN already registered.");
        }
        var newBook = _mapper.Map<BookModel>(bookDto);
        await _context.Books.AddAsync(newBook);
        var changes = await _context.SaveChangesAsync();

        return (changes > 0)
        ? new SuccessResult("Book registered successfully.") : new ErrorResult("Failed to register the book.");
    }

    public async Task<ResultModel> UpdateBook(string id, BookViewModel updatedDto)
    {
        var book = await GetBookById(id);
        if (book is ErrorResult)
        {
            return new ErrorResult($"No book found with ID: {id}");
        }

        var bookModel = ((SuccessDataResult<BookModel>)book).Data;
        _mapper.Map(updatedDto, bookModel);
        var changes = await _context.SaveChangesAsync();

        return (changes > 0)
        ? new SuccessResult("Book updated successfully.") : new ErrorResult("Failed to update the book.");
    }

    public async Task<ResultModel> DeleteBook(string id)
    {
        var book = await GetBookById(id);
        if (book is ErrorResult)
        {
            return new ErrorResult($"No book found with ID: {id}");
        }

        var bookModel = ((SuccessDataResult<BookModel>)book).Data;
        _context.Books.Remove(bookModel);
        var changes = await _context.SaveChangesAsync();

        return (changes > 0)
        ? new SuccessResult("Book deleted successfully.") : new ErrorResult("Failed to delete the book.");
    }
}
