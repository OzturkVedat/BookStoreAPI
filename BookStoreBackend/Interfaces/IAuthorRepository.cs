using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;

public interface IAuthorRepository
{
    Task<ResultModel> GetAuthorById(string id);
    Task<ResultModel> GetAllAuthors(int page, int pageSize);
    Task<int> GetAuthorCount();
    Task<ResultModel> RegisterAuthor(AuthorViewModel dto);
    Task<ResultModel> UpdateAuthor(string id, AuthorViewModel updatedDto);
    Task<ResultModel> DeleteAuthor(string id);
}
