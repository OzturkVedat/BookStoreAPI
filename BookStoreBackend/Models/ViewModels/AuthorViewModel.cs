using System.ComponentModel.DataAnnotations;

namespace BookStoreBackend.Models.ViewModels
{
    public class AuthorViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [StringLength(200, ErrorMessage = "Biography cannot exceed 200 characters.")]
        public string? Biography { get; set; }

        [StringLength(50, ErrorMessage = "Nationality cannot exceed 50 characters.")]
        public string? Nationality { get; set; }
    }

}
