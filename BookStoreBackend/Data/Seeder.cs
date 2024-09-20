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
                new AuthorModel { Id= "JLondon", FirstName= "Jack",LastName="London", Nationality= "American", Biography= "American writer..." },
                new AuthorModel { Id= "LCarroll", FirstName= "Lewis",LastName="Carroll",  Nationality= "British", Biography= "Author of the famous works Alice's Adventures in Wonderland and Through the Looking-Glass." },
                new AuthorModel { Id= "MTwain", FirstName= "Mark",LastName="Twain",  Nationality= "American", Biography= "American author known for his novels The Adventures of Tom Sawyer and Adventures of Huckleberry Finn." },
            };
            _context.Authors.AddRange(authors);
            await _context.SaveChangesAsync();
        }

        private async Task SeedBooks()
        {
            var books = new List<BookModel>
            {
                new BookModel { Id= "MEden", Title= "Martin Eden", BookGenre= Genre.Drama, Price= 20, AuthorId= "JLondon", 
                    Publisher = "Penguin Classics", PageCount = 464, BookLanguage = Language.English, ISBN = "9780140187734",
                    Description = "A semi-autobiographical novel about a writer's journey.", Stock = 103 },
            
                new BookModel { Id= "TCoW", Title= "The Call of the Wild", BookGenre= Genre.Adventure, Price= 15, AuthorId= "JLondon", 
                    Publisher = "Macmillan Publishers", PageCount = 232, BookLanguage = Language.English, ISBN = "9781503280465",
                    Description = "A classic adventure novel set during the Klondike Gold Rush.", Stock = 50 },
                
                new BookModel { Id= "AlicesAdv", Title= "Alice's Adventures in Wonderland", BookGenre= Genre.Fantasy, Price= 18, AuthorId= "LCarroll",
                    Publisher = "Macmillan Publishers", PageCount = 96, BookLanguage = Language.English, ISBN = "9781503222687",
                    Description = "A fantastical tale about a girl named Alice and her adventures.", Stock = 72 },
                
                new BookModel { Id= "TTSawyer", Title= "The Adventures of Tom Sawyer", BookGenre= Genre.Adventure, Price= 14, AuthorId= "MTwain",
                    Publisher = "Chatto & Windus", PageCount = 274, BookLanguage = Language.English, ISBN = "9780141439648",
                    Description = "The classic tale of Tom Sawyer and his adventures.", Stock = 80 },
            };
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();
        }

    }
}