using BookStoreBackend.Data;
using BookStoreBackend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

using Microsoft.EntityFrameworkCore;
using BookStoreBackend.Models.ViewModels;
using AutoMapper;
using System.ComponentModel;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public AuthorRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AuthorModel?> GetAuthorById(string id)
    {
        return await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AuthorModel>> GetAllAuthors()
    {
        var authors= await _context.Authors
                        .AsNoTracking()
                        .ToListAsync();
        if(authors != null && authors.Any())
        {
            return authors;
        }
        return Enumerable.Empty<AuthorModel>();
    }
    public async Task<int> GetAuthorCount()
    {
        return await _context.Authors.CountAsync();
    }

    public async Task RegisterAuthor(AuthorFullNameDto dto)
    {
        var newAuthor = _mapper.Map<AuthorModel>(dto);
        await _context.Authors.AddAsync(newAuthor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAuthor(string id, AuthorFullNameDto updatedDto)
    {
        var author = await GetAuthorById(id);
        if (author != null)
        {
            _mapper.Map(updatedDto, author);
            await _context.SaveChangesAsync();
        } 
    }

    public async Task DeleteAuthor(string id)
    {
        var author = await GetAuthorById(id);
        if (author != null)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }       
    }
}
