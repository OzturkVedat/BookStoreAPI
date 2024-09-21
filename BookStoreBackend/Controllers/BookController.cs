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
            return BadRequest(new ErrorResult("Invalid book ID."));

        var result = await _bookRepository.GetBookById(id);
        return (!result.IsSuccess) ? NotFound(result) : Ok(result);

    }

    [HttpGet("all-books")]
    public async Task<IActionResult> GetAllBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
    {
        if (page < 1)
        {
            return BadRequest(new ErrorResult("Page number must be greater than or equal to 1."));
        }

        if (pageSize < 1 || pageSize > 200)
        {
            return BadRequest(new ErrorResult("Page size must be between 1 and 100."));
        }
        var result = await _bookRepository.GetAllBooks(page, pageSize);
        return (!result.IsSuccess) ? NotFound(result) : Ok(result);
    }

    [HttpPost("register-book")]
    public async Task<IActionResult> RegisterBook(BookViewModel bookDto)
    {
        if (!ModelState.IsValid)
        {
            var error = new ErrorDataResult("Invalid input for book registration.", ModelState.GetErrors());
            return BadRequest(error);
        }
        var result = await _bookRepository.RegisterBook(bookDto);
        return (!result.IsSuccess) ? BadRequest(result) : Ok(result);
    }

    [HttpPut("update-book/{id}")]
    public async Task<IActionResult> UpdateBook(string id, BookViewModel bookDto)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            var error = new ErrorResult { Message = "Invalid book ID." };
            return BadRequest(error);
        }
        if (!ModelState.IsValid)
        {
            var error = new ErrorDataResult("Invalid inputs.", ModelState.GetErrors());
            return BadRequest(error);
        }
        var result = await _bookRepository.UpdateBook(id, bookDto);
        if (result.IsSuccess) 
            return Ok(result);

        return (result.Message == "Failed to update the book.") ? BadRequest(result) : NotFound(result);
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
        if (result.IsSuccess)
            return Ok(result);

        return (result.Message == $"No book found with ID: {id}") ? NotFound(result) : BadRequest(result);
    }
}
