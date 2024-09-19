using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;

public interface IBookRepository
{
    Task<BookModel> GetBookById(string id);
    Task<IEnumerable<BookModel>> GetAllBooks();
    Task<int> GetBookCount();
    Task<bool> RegisterBook(BookViewModel bookDto);
    Task<bool> UpdateBook(string id, BookViewModel bookDto);
    Task<bool> DeleteBook(string id);
}
