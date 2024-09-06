using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<BookController> _logger;

    public BookController(IBookRepository bookRepository, ILogger<BookController> logger)
    {
        _bookRepository = bookRepository;
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
            var book = await _bookRepository.GetBookById(id);
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
            var books = await _bookRepository.GetAllBooks();

            if (books == null || !books.Any())
            {
                var error = new ErrorResult { Message = "No books found." };
                return NotFound(error);
            }

            var success = new SuccessDataResult<IEnumerable<BookModel>>
            {
                Message = "Successfully fetched all books.",
                Data = books
            };

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
            await _bookRepository.RegisterBook(bookDto);

            var success = new SuccessResult { Message = "Book successfully created." };
            return Ok(success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the book.");
            var errorResult = new ErrorDataResult { Message = "An unexpected error occurred. Please try again later." };
            return StatusCode(500, errorResult);
        }
    }

    [HttpPut("update-book/{id}")]
    public async Task<IActionResult> UpdateBook(string id, BookViewModel bookDto)
    {
        if (!ModelState.IsValid)
        {
            var error = new ErrorDataResult { Message = "Invalid input for book update.", Errors = ModelState.GetErrors() };
            return BadRequest(error);
        }

        try
        {
            await _bookRepository.UpdateBook(id, bookDto);

            var success = new SuccessResult { Message = "Book successfully updated." };
            return Ok(success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the book.");
            var errorResult = new ErrorResult { Message = "An unexpected error occurred. Please try again later." };
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
            await _bookRepository.DeleteBook(id);

            var success = new SuccessResult { Message = "Book successfully deleted." };
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
