using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        var result = await _authorRepository.GetAuthorById(id);
        return (result.IsSuccess) ? Ok(result) : NotFound(result);
    }

    [HttpGet("all-authors")]
    public async Task<IActionResult> GetAllAuthors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1)
        {
            return BadRequest(new ErrorResult("Page number must be greater than or equal to 1."));
        }

        if (pageSize < 1 || pageSize > 200)
        {
            return BadRequest(new ErrorResult("Page size must be between 1 and 100."));
        }
        var result = await _authorRepository.GetAllAuthors(page, pageSize);
        return (result.IsSuccess) ? Ok(result) : NotFound(result);
    }

    [HttpPost("register-author")]
    public async Task<IActionResult> RegisterAuthor(AuthorViewModel authorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult("Invalid input for author registration", ModelState.GetErrors()));
        }
        var result = await _authorRepository.RegisterAuthor(authorDto);
        return (result.IsSuccess) ? Ok(result) : BadRequest(result);
    }

    [HttpPut("update-author/{id}")]
    public async Task<IActionResult> UpdateAuthor(string id, AuthorViewModel authorDto)
    {
        if (string.IsNullOrWhiteSpace(id) || !ModelState.IsValid)
        {
            return BadRequest(new ErrorDataResult("Invalid input for author update", ModelState.GetErrors()));
        }
        var result = await _authorRepository.UpdateAuthor(id, authorDto);
        if (result.IsSuccess)
            return Ok(result);
        return (result.Message == "Failed to update the author details.")
         ? BadRequest(result) : NotFound(result);
    }

    [HttpDelete("delete-author/{id}")]
    public async Task<IActionResult> DeleteAuthor(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResult { Message = "Invalid author ID." });

        var result = await _authorRepository.DeleteAuthor(id);
        if (result.IsSuccess)
            return Ok(result);
        return (result.Message == " Failed to remove the author record.")
         ? BadRequest(result) : NotFound(result);
    }
}
