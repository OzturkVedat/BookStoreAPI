using BookStoreBackend.Models;

namespace BookStoreBackend.Data
{
    public class Seeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Seeder> _logger;
        public Seeder(ApplicationDbContext context, ILogger<Seeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedDbContext()
        {
            try
            {
                if (!_context.Authors.Any())
                {
                    await SeedAuthors();
                }
                if (!_context.Books.Any())
                {
                    await SeedBooks();
                }
                else
                {
                    _logger.LogInformation("Database already seeded.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while seeding data.");
            }
        }

        private async Task SeedAuthors()
        {
            var authors = new List<AuthorModel>
            {
                new AuthorModel {Id= "JLondon", FullName= "Jack London", Nationality= "American", Biography= "American writer..." },
                new AuthorModel { Id= "LCarroll", FullName= "Lewis Carroll", Nationality= "British", Biography= "Author of the famous works Alice's Adventures in Wonderland and Through the Looking-Glass." },
                new AuthorModel { Id= "MTwain", FullName= "Mark Twain", Nationality= "American", Biography= "American author known for his novels The Adventures of Tom Sawyer and Adventures of Huckleberry Finn." },
            };
            _context.Authors.AddRange(authors);
            await _context.SaveChangesAsync();
        }

        private async Task SeedBooks()
        {
            var books = new List<BookModel>
            {
                new BookModel{Id= "MEden", Title= "Martin Eden",BookGenre=Genre.Drama,Price=20,AuthorId="JLondon"},
                new BookModel { Id= "TCoW", Title= "The Call of the Wild", BookGenre= Genre.Adventure, Price= 15, AuthorId= "JLondon" },
                new BookModel { Id= "AlicesAdv", Title= "Alice's Adventures in Wonderland", BookGenre= Genre.Fantasy, Price= 18, AuthorId= "LCarroll" },
                new BookModel { Id= "TTSawyer", Title= "The Adventures of Tom Sawyer", BookGenre= Genre.Adventure, Price= 14, AuthorId= "MTwain" },
            };
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
        }
    }
}