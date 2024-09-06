using BookStoreBackend.Data;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BookModel> GetBookById(string id)
    {
        return await _context.Books
                             .FromSqlInterpolated($"EXECUTE dbo.books_GetById {id}")
                             .AsNoTracking()
                             .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BookModel>> GetAllBooks()
    {
        return await _context.Books
                             .FromSqlInterpolated($"EXECUTE dbo.books_GetAll")
                             .AsNoTracking()
                             .ToListAsync();
    }

    public async Task RegisterBook(BookViewModel bookDto)
    {
        var parameters = new[]      // parameterizing to prevent sql injections
        {
            new SqlParameter("@Title", SqlDbType.NVarChar, 100) { Value = (object)bookDto.Title ?? DBNull.Value },
            new SqlParameter("@Genre", SqlDbType.Int) { Value = (int)bookDto.BookGenre },
            new SqlParameter("@Price", SqlDbType.Int) { Value = bookDto.Price },
            new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = (object)bookDto.AuthorId ?? DBNull.Value }
        };

        await _context.Database.ExecuteSqlRawAsync("EXEC dbo.books_Register @Title, @Genre, @Price, @AuthorId", parameters);
    }

    public async Task UpdateBook(string id, BookViewModel bookDto)
    {
        var parameters = new[]
        {
            new SqlParameter("@BookId", SqlDbType.NVarChar, 50) { Value = id },
            new SqlParameter("@Title", SqlDbType.NVarChar, 100) { Value = (object)bookDto.Title ?? DBNull.Value },
            new SqlParameter("@Genre", SqlDbType.Int) { Value = (int)bookDto.BookGenre },
            new SqlParameter("@Price", SqlDbType.Int) { Value = bookDto.Price },
            new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = (object)bookDto.AuthorId ?? DBNull.Value }
        };

        await _context.Database.ExecuteSqlRawAsync("EXEC dbo.books_Update @BookId, @Title, @Genre, @Price, @AuthorId", parameters);
    }

    public async Task DeleteBook(string id)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.books_Delete {id}");
    }
}
