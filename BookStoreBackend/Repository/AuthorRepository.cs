using BookStoreBackend.Data;
using BookStoreBackend.Models;
using Microsoft.EntityFrameworkCore;
using BookStoreBackend.Models.ViewModels;
using AutoMapper;

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
        return await _context.Authors
                        .AsNoTracking()
                        .ToListAsync();
    }
    public async Task<int> GetAuthorCount()
    {
        return await _context.Authors.CountAsync();
    }

    public async Task<bool> RegisterAuthor(AuthorFullNameDto dto)
    {
        var newAuthor = _mapper.Map<AuthorModel>(dto);
        await _context.Authors.AddAsync(newAuthor);
        var changes= await _context.SaveChangesAsync();
        return (changes > 0);
    }

    public async Task<bool> UpdateAuthor(string id, AuthorFullNameDto updatedDto)
    {
        var author = await GetAuthorById(id);
        if (author != null)
        {
            _mapper.Map(updatedDto, author);
            var changes= await _context.SaveChangesAsync();
            return (changes > 0);
        }
        return false;
    }

    public async Task<bool>DeleteAuthor(string id)
    {
        var author = await GetAuthorById(id);
        if (author != null)
        {
            _context.Authors.Remove(author);
            var changes= await _context.SaveChangesAsync();
            return (changes > 0);
        }
        return false;
    }
}
