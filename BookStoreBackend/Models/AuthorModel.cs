using BookStoreBackend.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStoreBackend.Models
{
    [Table("Authors", Schema = "dbo")]
    public class AuthorModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }  

        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public string? Biography { get; set; }
        public string? Nationality { get; set; }
        public ICollection<BookModel>? Books { get; set; }

        public AuthorModel() {
            Id = Guid.NewGuid().ToString();
            Books = new List<BookModel>();
        }
        public AuthorModel(string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString();
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Books = new List<BookModel>(); 
        }
    }
}
