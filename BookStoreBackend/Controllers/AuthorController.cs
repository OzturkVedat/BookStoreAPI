using BookStoreBackend.Data;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStoreBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {

        private readonly ILogger<AuthorController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthorController(ILogger<AuthorController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("author-details/{id}")]
        public async Task<IActionResult> GetAuthorById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var error = new ErrorResult { Message = "Invalid author ID." };
                return BadRequest(error);
            }
            try
            {
                var author = await _context.Authors
                                       .FromSqlInterpolated($"EXECUTE dbo.authors_GetById {id}")  // safe parameter interpolation
                                       .AsNoTracking()  // improve performance for read-only queries
                                       .FirstOrDefaultAsync();
                if (author == null)
                {
                    var error = new ErrorResult { Message = "Author not found." };
                    return NotFound(error);
                }

                var success = new SuccessDataResult<AuthorModel>
                {
                    Message = "Successfully fetched the details of the author.",
                    Data = author
                };

                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching author details..");
                var error = new ErrorResult { Message = "An unexpected error occurred.." };
                return StatusCode(500, error);
            }

        }


        [HttpGet("all-authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var authors = await _context.Authors
                                                .FromSqlInterpolated($"EXECUTE dbo.authors_GetAll")
                                                .AsNoTracking()
                                                .ToListAsync();

                if (authors == null || !authors.Any())
                {
                    var error = new ErrorResult { Message = "No authors found.." };
                    return NotFound(error);
                }
                var success = new SuccessDataResult<IEnumerable<AuthorModel>>()
                { Message = "Successfully fetched all authors..", Data = authors };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching authors..");
                var error = new ErrorResult { Message = "An unexpected error occured.." };
                return StatusCode(500, error);
            }

        }

        [HttpPost("register-author")]
        public async Task<IActionResult> RegisterAuthor(AuthorViewModel authorDto)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorDataResult
                { Message = "Invalid input for author registration", Errors = ModelState.GetErrors() };
                return BadRequest(error);
            }

            try
            {
                string FullName = $"{authorDto.FirstName} {authorDto.LastName}";
                await _context.Database.
                    ExecuteSqlInterpolatedAsync($"EXECUTE dbo.authors_Register {FullName}");

                var success = new SuccessResult
                { Message = "Author successfully registered." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the author.");
                var errorResult = new ErrorDataResult
                { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }

        [HttpPost("register-author-by-fullname")]
        public async Task<IActionResult> RegisterAuthor(AuthorFullNameDto authorDto)
        {
            if (!ModelState.IsValid)
            {
                var error = new ErrorDataResult
                { Message = "Invalid input for author registration", Errors = ModelState.GetErrors() };
                return BadRequest(error);
            }

            try
            {
                // Create parameters for the stored procedure
                var parameters = new[]
                {
            new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = (object)authorDto.FullName ?? DBNull.Value },
            new SqlParameter("@Biography", SqlDbType.NVarChar, 500) { Value = (object)authorDto.Biography ?? DBNull.Value },
            new SqlParameter("@Nationality", SqlDbType.NVarChar, 50) { Value = (object)authorDto.Nationality ?? DBNull.Value }
        };

                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Register @FullName, @Biography, @Nationality", parameters);

                var success = new SuccessResult
                { Message = "Author successfully registered." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the author.");
                var errorResult = new ErrorResult
                { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }

        [HttpPut("update-author/{id}")]
        public async Task<IActionResult> UpdateAuthor(string id, AuthorViewModel authorDto)
        {
            if (string.IsNullOrWhiteSpace(id) || !ModelState.IsValid)
            {
                var error = new ErrorDataResult
                { Message = "Invalid input for author update", Errors = ModelState.GetErrors() };
                return BadRequest(error);
            }

            try
            {
                string FullName = $"{authorDto.FirstName} {authorDto.LastName}";

                var parameters = new[]
                {
            new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = id },
            new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = FullName },
            new SqlParameter("@Biography", SqlDbType.NVarChar, 500) { Value = (object)authorDto.Biography ?? DBNull.Value },
            new SqlParameter("@Nationality", SqlDbType.NVarChar, 50) { Value = (object)authorDto.Nationality ?? DBNull.Value }
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Update @AuthorId, @FullName, @Biography, @Nationality", parameters);

                var success = new SuccessResult { Message = "Author successfully updated." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the author.");
                var errorResult = new ErrorDataResult
                { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }
        [HttpDelete("delete-author/{id}")]
        public async Task<IActionResult> DeleteAuthor(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var error = new ErrorResult { Message = "Invalid author ID." };
                return BadRequest(error);
            }

            try
            {
                var parameter = new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = id };
                await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Delete @AuthorId", parameter);

                var success = new SuccessResult { Message = "Author successfully deleted." };
                return Ok(success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the author.");
                var errorResult = new ErrorResult { Message = "An unexpected error occurred. Please try again later." };
                return StatusCode(500, errorResult);
            }
        }

    }
}
