using BookStoreBackend.Models;

namespace BookStoreBackend.Models
{
    public class BookModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public Genre BookGenre { get; set; }
        public int Price { get; set; }
        public string AuthorId { get; set; }
        public AuthorModel Author { get; set; }
    }
    public enum Genre
    {
        Literary,
        Fantasy,
        Horror,
        Drama,
        SciFi,
        Romance,
        Mystery,
        Adventure,
        Dystopian,
        Biography,
        Philosophy,
        Poetry,
        History,
        Memoir,
        Politics,
        Health
    }
}
