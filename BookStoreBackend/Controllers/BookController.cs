using BookStoreBackend.Data;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookStoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookController> _logger;
        public BookController(ApplicationDbContext context, ILogger<BookController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet("book-details/{id}")]
        public async Task<IActionResult> GetBookById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var error = new ErrorResult { Message = "Invalid book ID." };
                return BadRequest(error);
            }
            try
            {
                var book = await _context.Books
                                       .FromSqlInterpolated($"EXECUTE dbo.books_GetById {id}")
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync();
                if (book == null)
                {
                    var error = new ErrorResult { Message = "Book not found." };
                    return NotFound(error);
                }

                var success = new SuccessDataResult<BookModel>
                {
                    Message = "Successfully fetched the book details.",
                    Data = book
                };

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching book details.");
                var error = new ErrorResult { Message = "An unexpected error occurred." };
                return StatusCode(500, error);
            }
        }

        [HttpGet("all-books")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _context.Books
                                          .FromSqlInterpolated($"EXECUTE dbo.books_GetAll")
                                          .AsNoTracking()
                                          .ToListAsync();

                if (books == null || !books.Any())
                {
                    var error = new ErrorResult { Message = "No books found." };
                    return NotFound(error);
                }
                var success = new SuccessDataResult<IEnumerable<BookModel>>()
                { Message = "Successfully fetched all books.", Data = books };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books.");
                var error = new ErrorResult { Message = "An unexpected error occurred." };
                return StatusCode(500, error);
            }
        }
        [HttpPost("register-book")]
        public async Task<IActionResult> RegisterBook(BookViewModel bookDto)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorDataResult
                { Message = "Invalid input for book creation.", Errors = ModelState.GetErrors() };
                return BadRequest(error);
            }

            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Title", SqlDbType.NVarChar, 100) { Value = (object)bookDto.Title ?? DBNull.Value },
                    new SqlParameter("@Genre", SqlDbType.Int) { Value = (int)bookDto.BookGenre },
                    new SqlParameter("@Price", SqlDbType.Int) { Value = bookDto.Price },
                    new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = (object)bookDto.AuthorId ?? DBNull.Value }
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.books_Register @Title, @Genre, @Price, @AuthorId", parameters);

                var success = new SuccessResult
                { Message = "Book successfully created." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the book.");
                var errorResult = new ErrorDataResult
                { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }

        // Update Book
        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> UpdateBook(string id, BookViewModel bookDto)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorDataResult
                { Message = "Invalid input for book update.", Errors = ModelState.GetErrors() };
                return BadRequest(error);
            }

            try
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

                var success = new SuccessResult
                { Message = "Book successfully updated." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the book.");
                var errorResult = new ErrorResult
                { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }

        [HttpDelete("delete-book/{id}")]
        public async Task<IActionResult> DeleteBook(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var error = new ErrorResult { Message = "Invalid book ID." };
                return BadRequest(error);
            }

            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.books_Delete {id}");

                var success = new SuccessResult
                { Message = "Book successfully deleted." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the book.");
                var error = new ErrorResult { Message = "An unexpected error occurred." };
                return StatusCode(500, error);
            }
        }
    }
}
