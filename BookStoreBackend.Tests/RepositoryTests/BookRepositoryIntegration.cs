using AutoMapper;
using BookStoreBackend.Data;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.RepositoryTests
{
    public class BookRepositoryIntegration: IClassFixture<IntegrationTestFixture>
    {
        private readonly BookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public BookRepositoryIntegration(IntegrationTestFixture fixture)
        {
            _context = fixture.context;
            _mapper = fixture.mapper;
            _bookRepository= new BookRepository(_context,_mapper);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnTheBook_WhenExists()
        {
            // ARRANGE
            var bookId = "MEden";       // an actual seeded book ID

            // ACT
            var result= await _bookRepository.GetBookById(bookId);

            // ASSERT
            result.Should().BeOfType<SuccessDataResult<BookModel>>();
            var successResult = result as SuccessDataResult<BookModel>; // casting
            successResult.Data.Title.Should().Be("Martin Eden");
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNull_WhenNotExists()
        {
            // ACT
            var result = await _bookRepository.GetBookById("not-existing-id");

            // ASSERT
            result.Should().BeOfType<ErrorResult>();
            var errorResult = result as ErrorResult;
            errorResult.Message.Should().Be("No book found with ID: not-existing-id");
        }
        [Fact]
        public async Task GetBooksByTitle_ShouldReturnBooks_WhenExist()
        {
            // Arrange
            var bookTitle = "The Call of the Wild";     // another seeded book

            // Act
            var result = await _bookRepository.GetBooksByTitle(bookTitle);

            // Assert
            result.Should().BeOfType<SuccessDataResult<IEnumerable<BookModel>>>();
            var successResult = result as SuccessDataResult<IEnumerable<BookModel>>;
            successResult.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnPaginated_IfExists()
        {
            // ARRANGE
            int page = 1;
            int pageSize = 4;

            // ACT
            var result= await _bookRepository.GetAllBooks(page, pageSize);

            // ASSERT
            result.Should().BeOfType<SuccessDataResult<IEnumerable<BookModel>>>();
            var successResult= result as SuccessDataResult<IEnumerable<BookModel>>;
            successResult.Data.Should().HaveCount(pageSize);   // 4 seeded books we have
            successResult.Data.Should().Contain(b => b.Title == "Martin Eden");
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnEmpty_WhenNoBooksInPage()
        {
            // Arrange
            int page = 2;
            int pageSize = 5;

            // Act: (requesting a second page, which have no books)
            var result = await _bookRepository.GetAllBooks(page, pageSize);

            // Assert
            result.Should().BeOfType<ErrorResult>();
            var errorResult = result as ErrorResult;
            errorResult.Message.Should().Be("No books found for the requested page.");
        }

        [Fact]
        public async Task GetBookCount_ShouldReturnCorrectCount()
        {
            // ACT
            var count = await _bookRepository.GetBookCount();

            // ASSERT
            count.Should().BeGreaterThanOrEqualTo(4);   // 4 books in the seed data, at least
        }

        [Fact]
        public async Task RegisterBook_ShouldAddNewBook()
        {
            // ARRANGE
            var newBook = new BookViewModel
            {
                Title = "White Fang",
                BookGenre = Genre.Literary,
                Price = 25,
                AuthorId = "JLondon", // existing author
                BookLanguage= Language.English,
                Publisher= "Hachette",
                PageCount= 400
            };

            // ACT
            var result = await _bookRepository.RegisterBook(newBook);
            var bookCount = await _bookRepository.GetBookCount();

            // ASSERT
            result.Should().BeOfType<SuccessResult>();
            var successResult= result as SuccessResult;
            bookCount.Should().BeGreaterThanOrEqualTo(5); // now there should be 5 books
        }
        [Fact]
        public async Task RegisterBook_ShouldNotAllowDuplicate()
        {
            // ARRANGE
            var duplicateBook = new BookViewModel
            {
                Title = "Martin Eden",
                BookGenre = Genre.Literary,
                Price = 100,
                AuthorId = "Jlondon",
                ISBN = "9780140187734",      // already seeded ISBN
                Publisher = "Hachette",
                PageCount = 250,
                BookLanguage = Language.English
            };

            // ACT
            var result= await _bookRepository.RegisterBook(duplicateBook);

            // ASSERT
            var errorResult = result as ErrorResult;
            errorResult.Should().NotBeNull();       // means casting is succeed
        }

        [Fact]
        public async Task UpdateBook_ShouldModifyExisting()
        {
            // ARRANGE
            string bookId = "AlicesAdv";
            var updatedDto = new BookViewModel
            {
                Title = "Alice's Adventures in Wonderland",
                BookGenre = Genre.Fantasy,
                Price = 20,     // changed 
                AuthorId = "LCarroll",
                Publisher = "Macmillan Publishers",
                PageCount = 96,
                BookLanguage = Language.English,
                ISBN = "9781503222687",
                Description = "A fantastical tale about a girl named Alice and her adventures.",
                Stock = 55      // changed
            };

            // ACT
            var result= await _bookRepository.UpdateBook(bookId, updatedDto);
            var updatedBook = await _bookRepository.GetBookById(bookId);

            // ASSERT
            result.Should().BeOfType<SuccessResult>();

            var book = updatedBook as SuccessDataResult<BookModel>;
            book.Should().NotBeNull();
            book.Data.Stock.Should().Be(55);
            book.Data.Price.Should().Be(20);
        }

        [Fact]
        public async Task DeleteBook_ShouldRemove_WhenExists()
        {
            // ARRANGE
            var books = await _bookRepository.GetBooksByTitle("Martin Eden");
            var booksResult= books as SuccessDataResult<IEnumerable<BookModel>>;
            var registeredBook = booksResult.Data.FirstOrDefault();

            // ACT & ASSERT
            var result= await _bookRepository.DeleteBook(registeredBook.Id);
            result.Should().BeOfType<SuccessResult>();

            var checkResult = await _bookRepository.GetBookById(registeredBook.Id);
            checkResult.Should().BeOfType<ErrorResult>();
        }
    }
}
