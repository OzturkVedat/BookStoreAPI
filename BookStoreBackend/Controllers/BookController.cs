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

    [HttpGet("all-books")]
    public async Task<IActionResult> GetAllBooks()
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

    [HttpPost("register-book")]
    public async Task<IActionResult> RegisterBook(BookViewModel bookDto)
    {
        if (!ModelState.IsValid)
        {
            var error = new ErrorDataResult
            { Message = "Invalid input for book registration.", Errors = ModelState.GetErrors() };
            return BadRequest(error);
        }
        var result= await _bookRepository.RegisterBook(bookDto);
        if (result)
            return Ok(new SuccessResult { Message = "Book successfully created." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to register the book for given inputs. Sorry?" });
    }

    [HttpPut("update-book/{id}")]
    public async Task<IActionResult> UpdateBook(string id, BookViewModel bookDto)
    {
        if (!ModelState.IsValid)
        {
            var error = new ErrorDataResult { Message = "Invalid input for book update.", Errors = ModelState.GetErrors() };
            return BadRequest(error);
        }


        var result = await _bookRepository.UpdateBook(id, bookDto);
        if (result)
            return Ok(new SuccessResult { Message = "Book successfully updated." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to update the details for the requested book" });

    }

    [HttpDelete("delete-book/{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            var error = new ErrorResult { Message = "Invalid book ID." };
            return BadRequest(error);
        }
        var result = await _bookRepository.DeleteBook(id);
        if (result)
            return Ok(new SuccessResult { Message = "Book successfully deleted." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to delete the book record for the given ID" });
    }
}
