using BookStoreBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStoreBackend.Models
{
    [Table("Authors", Schema = "dbo")]
    public class AuthorModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string? Biography { get; set; }
        public string? Nationality { get; set; }
        public ICollection<BookModel>? Books { get; set; } = new List<BookModel>();

        public AuthorModel(string fullName) {
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
        }
        public AuthorModel(string firstName, string lastName)   // constructor overload
        {
            Id = Guid.NewGuid().ToString();
            FullName = $"{firstName} {lastName}";
        }
    }
}
