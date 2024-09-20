using System.ComponentModel.DataAnnotations;

namespace BookStoreBackend.Models.ViewModels
{
    public class BookViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public Genre BookGenre { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive value")]
        public int Price { get; set; }
        [Required]
        public string Publisher { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Page count must be at least 1")]
        public int PageCount { get; set; }
        [Required]
        public Language BookLanguage { get; set; }
        [Required]
        public string AuthorId { get; set; }

        // optionals
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
    }
}
