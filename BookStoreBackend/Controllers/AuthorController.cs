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

        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching author details.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred." });
        }
    }

    [HttpGet("all-authors")]
    public async Task<IActionResult> GetAllAuthors()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching authors.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred." });
        }
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

        try
        {
            string fullName = $"{authorDto.FirstName} {authorDto.LastName}";
            AuthorFullNameDto dto= new AuthorFullNameDto()
            {
                FullName = fullName,
                Nationality= authorDto.Nationality,
                Biography= authorDto.Biography
            };
            await _authorRepository.RegisterAuthor(dto);

            return Ok(new SuccessResult { Message = "Author successfully registered." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering the author.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpPost("register-author-by-fullname")]
    public async Task<IActionResult> RegisterAuthor(AuthorFullNameDto authorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult
            {
                Message = "Invalid input for author registration",
                Errors = ModelState.GetErrors()
            });
        }

        try
        {
            await _authorRepository.RegisterAuthor(authorDto);

            return Ok(new SuccessResult { Message = "Author successfully registered." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering the author.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred. Please try again later." });
        }
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

        try
        {
            await _authorRepository.UpdateAuthor(id, authorDto);
            return Ok(new SuccessResult { Message = "Author successfully updated." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the author.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpDelete("delete-author/{id}")]
    public async Task<IActionResult> DeleteAuthor(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new ErrorResult { Message = "Invalid author ID." });
        }

        try
        {
            await _authorRepository.DeleteAuthor(id);
            return Ok(new SuccessResult { Message = "Author successfully deleted." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the author.");
            return StatusCode(500, new ErrorResult { Message = "An unexpected error occurred. Please try again later." });
        }
    }
}
