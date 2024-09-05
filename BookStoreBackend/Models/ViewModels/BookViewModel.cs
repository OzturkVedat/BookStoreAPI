namespace BookStoreBackend.Models.ViewModels
{
    public class BookViewModel
    {
        public string Title { get; set; }
        public Genre BookGenre { get; set; }
        public int Price { get; set; }
        public string AuthorId {  get; set; }
    }
}
