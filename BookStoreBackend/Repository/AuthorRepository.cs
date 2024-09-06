using BookStoreBackend.Data;
using BookStoreBackend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

using Microsoft.EntityFrameworkCore;
using BookStoreBackend.Models.ViewModels;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorModel?> GetAuthorById(string id)
    {
        return await _context.Authors
            .FromSqlInterpolated($"EXECUTE dbo.authors_GetById {id}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AuthorModel>> GetAllAuthors()
    {
        return await _context.Authors
            .FromSqlInterpolated($"EXECUTE dbo.authors_GetAll")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RegisterAuthor(AuthorFullNameDto dto)
    {
        var parameters = new[]
        {
            new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = dto.FullName },
            new SqlParameter("@Biography", SqlDbType.NVarChar, 500) { Value = (object)dto.Biography ?? DBNull.Value },
            new SqlParameter("@Nationality", SqlDbType.NVarChar, 50) { Value = (object)dto.Nationality ?? DBNull.Value }
        };

        await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Register @FullName, @Biography, @Nationality", parameters);
    }

    public async Task UpdateAuthor(string id, AuthorFullNameDto updatedDto)
    {
        var parameters = new[]
        {
            new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = id },
            new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = updatedDto.FullName },
            new SqlParameter("@Biography", SqlDbType.NVarChar, 500) { Value = (object)updatedDto.Biography ?? DBNull.Value },
            new SqlParameter("@Nationality", SqlDbType.NVarChar, 50) { Value = (object)updatedDto.Nationality ?? DBNull.Value }
        };

        await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Update @AuthorId, @FullName, @Biography, @Nationality", parameters);
    }

    public async Task DeleteAuthor(string id)
    {
        var parameter = new SqlParameter("@AuthorId", SqlDbType.NVarChar, 50) { Value = id };
        await _context.Database.ExecuteSqlRawAsync("EXEC dbo.authors_Delete @AuthorId", parameter);
    }
}
