using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthorController : ControllerBase
{
    private readonly ILogger<AuthorController> _logger;
    private readonly IAuthorRepository _authorRepository;

    public AuthorController(ILogger<AuthorController> logger, IAuthorRepository authorRepository)
    {
        _logger = logger;
        _authorRepository = authorRepository;
    }

    [HttpGet("author-details/{id}")]
    public async Task<IActionResult> GetAuthorById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ErrorResult { Message = "Invalid author ID." });
        }
        var author = await _authorRepository.GetAuthorById(id);
        if (author == null)
        {
            return NotFound(new ErrorResult { Message = "Author not found." });
        }

        return Ok(new SuccessDataResult<AuthorModel>
        {
            Message = "Successfully fetched the details of the author.",
            Data = author
        });
    }

    [HttpGet("all-authors")]
    public async Task<IActionResult> GetAllAuthors()
    {

        var authors = await _authorRepository.GetAllAuthors();
        if (authors == null || !authors.Any())
        {
            return NotFound(new ErrorResult { Message = "No authors found." });
        }

        return Ok(new SuccessDataResult<IEnumerable<AuthorModel>>
        {
            Message = "Successfully fetched all authors.",
            Data = authors
        });
    }

    [HttpPost("register-author")]
    public async Task<IActionResult> RegisterAuthor(AuthorViewModel authorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult
            {
                Message = "Invalid input for author registration",
                Errors = ModelState.GetErrors()
            });
        }
        string fullName = $"{authorDto.FirstName} {authorDto.LastName}";
        AuthorFullNameDto dto = new AuthorFullNameDto()
        {
            FullName = fullName,
            Nationality = authorDto.Nationality,
            Biography = authorDto.Biography
        };
        var result = await _authorRepository.RegisterAuthor(dto);
        if (result)
            return Ok(new SuccessResult { Message = "Author successfully registered." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to register the author for given inputs." });
    }

    [HttpPost("register-author-by-fullname")]
    public async Task<IActionResult> RegisterAuthorByFullName(AuthorFullNameDto authorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult
            {
                Message = "Invalid input for author registration",
                Errors = ModelState.GetErrors()
            });
        }
        var result = await _authorRepository.RegisterAuthor(authorDto);
        if (result)
            return Ok(new SuccessResult { Message = "Author successfully registered." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to register the author for given inputs." });
    }

    [HttpPut("update-author/{id}")]
    public async Task<IActionResult> UpdateAuthor(string id, AuthorFullNameDto authorDto)
    {
        if (string.IsNullOrWhiteSpace(id) || !ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult
            {
                Message = "Invalid input for author update",
                Errors = ModelState.GetErrors()
            });
        }
        var result = await _authorRepository.UpdateAuthor(id, authorDto);
        if (result)
            return Ok(new SuccessResult { Message = "Author successfully updated." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to update the author for given inputs." });
    }

    [HttpDelete("delete-author/{id}")]
    public async Task<IActionResult> DeleteAuthor(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ErrorResult { Message = "Invalid author ID." });
        }
        var result = await _authorRepository.DeleteAuthor(id);
        if (result)
            return Ok(new SuccessResult { Message = "Author successfully deleted." });
        else
            return BadRequest(new ErrorResult { Message = "Failed to delete the author record for the given ID" });
    }
}
