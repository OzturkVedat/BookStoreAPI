using BookStoreBackend.Models;
using BookStoreBackend.Models.ViewModels;

public interface IAuthorRepository
{
    Task<AuthorModel?> GetAuthorById(string id);
    Task<IEnumerable<AuthorModel>> GetAllAuthors();
    Task<int> GetAuthorCount();
    Task RegisterAuthor(AuthorFullNameDto dto);
    Task UpdateAuthor(string id,AuthorFullNameDto updatedDto);
    Task DeleteAuthor(string id);
}
