using BookStoreBackend.Data;
using BookStoreBackend.Models;
using Microsoft.EntityFrameworkCore;
using BookStoreBackend.Models.ViewModels;
using AutoMapper;
using BookStoreBackend.Models.ResultModels;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public AuthorRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResultModel> GetAuthorById(string id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(b => b.Id == id);
        if (author == null)
        {
            return new ErrorResult($"No author found with ID: {id}");
        }
        return new SuccessDataResult<AuthorModel>("Author details retrieved successfully.", author);
    }

    public async Task<ResultModel> GetAllAuthors(int page, int pageSize)   // paginated
    {
        var authors = await _context.Authors
                             .AsNoTracking()
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
        return (authors.Any())
        ? new SuccessDataResult<IEnumerable<AuthorModel>>("Successfully fetched the requested authors.", authors)
        : new ErrorResult("No authors found for the requested page.");
    }

    public async Task<int> GetAuthorCount()
    {
        return await _context.Authors.CountAsync();
    }

    public async Task<ResultModel> RegisterAuthor(AuthorViewModel dto)
    {
        var newAuthor = _mapper.Map<AuthorModel>(dto);
        var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.FirstName == newAuthor.FirstName && a.LastName == newAuthor.LastName);

        if (existingAuthor != null)
            return new ErrorResult($"Author with name {newAuthor.FirstName} {newAuthor.LastName} is already registered.");

        await _context.Authors.AddAsync(newAuthor);
        var changes = await _context.SaveChangesAsync();
        return changes > 0
        ? new SuccessResult("Author registered successfully.") : new ErrorResult("Failed to register.");
    }

    public async Task<ResultModel> UpdateAuthor(string id, AuthorViewModel updatedDto)
    {
        var author = await GetAuthorById(id);
        if (author is ErrorResult)
            return new ErrorResult($"No author found with ID: {id}");

        var authorModel = ((SuccessDataResult<AuthorModel>)author).Data;
        _mapper.Map(updatedDto, authorModel);
        var changes= await _context.SaveChangesAsync();
        return changes > 0
        ? new SuccessResult("Author details updated successfully.") : new ErrorResult("Failed to update the author details."); 
    }

    public async Task<ResultModel> DeleteAuthor(string id)
    {
        var author = await GetAuthorById(id);
        if (author is ErrorResult)
            return new ErrorResult($"No author found with ID: {id}");

        var authorModel = ((SuccessDataResult<AuthorModel>)author).Data;
        _context.Authors.Remove(authorModel);
        var changes = await _context.SaveChangesAsync();

        return (changes > 0)
        ? new SuccessResult("Author removed successfully.") : new ErrorResult("Failed to remove the author record.");
    }
}
