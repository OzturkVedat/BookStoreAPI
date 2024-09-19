using BookStoreBackend.Models;
using BookStoreBackend.Models.ViewModels;

public interface IAuthorRepository
{
    Task<AuthorModel?> GetAuthorById(string id);
    Task<IEnumerable<AuthorModel>> GetAllAuthors();
    Task<int> GetAuthorCount();
    Task<bool> RegisterAuthor(AuthorFullNameDto dto);
    Task<bool> UpdateAuthor(string id,AuthorFullNameDto updatedDto);
    Task<bool> DeleteAuthor(string id);
}
