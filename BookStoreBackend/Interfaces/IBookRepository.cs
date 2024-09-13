using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Models;

public interface IBookRepository
{
    Task<BookModel> GetBookById(string id);
    Task<IEnumerable<BookModel>> GetAllBooks();
    Task<int> GetBookCount();
    Task RegisterBook(BookViewModel bookDto);
    Task UpdateBook(string id, BookViewModel bookDto);
    Task DeleteBook(string id);
}
