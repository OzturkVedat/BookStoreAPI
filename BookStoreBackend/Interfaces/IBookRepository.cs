using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;

public interface IBookRepository
{
    Task<ResultModel> GetBookById(string id);
    Task<ResultModel> GetBooksByTitle(string title);
    Task<ResultModel> GetAllBooks(int page, int pageSize);
    Task<int> GetBookCount();
    Task<bool> BookWithSameIsbnExists(string isbn);
    Task<ResultModel> RegisterBook(BookViewModel bookDto);
    Task<ResultModel> UpdateBook(string id, BookViewModel bookDto);
    Task<ResultModel> DeleteBook(string id);
}
