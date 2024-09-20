using System.ComponentModel.DataAnnotations;

namespace BookStoreBackend.Models
{
    public class BookModel
    {
        // non-nullable
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public Genre BookGenre { get; set; }
        public int Price { get; set; }    
        public string Publisher { get; set; }
        public int PageCount { get; set; }
        public Language BookLanguage { get; set; }
        public string AuthorId { get; set; }
        public AuthorModel Author { get; set; }

        // nullable
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "Invalid ISBN format")]
        public string? ISBN { get; set; }       // not all books have ISBN
        public string? Description { get; set; }
        public int? Stock { get; set; }

    }
    public enum Language
    {
        English,
        Spanish,
        French,
        German,
        Chinese,
        Japanese,
        Russian,
        Italian,
        Portuguese,
        Arabic,
        Turkish,
        // ...
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
        // ...
    }
}
