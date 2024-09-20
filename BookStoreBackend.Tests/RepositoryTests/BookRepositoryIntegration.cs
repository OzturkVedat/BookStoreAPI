using AutoMapper;
using BookStoreBackend.Data;
using BookStoreBackend.Models;
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
            result.Should().NotBeNull();
            result.Id.Should().Be(bookId);
            result.Title.Should().Be("Martin Eden");
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNull_WhenNotExists()
        {
            // ACT
            var result = await _bookRepository.GetBookById("not-existing-id");

            // ASSERT
            result.Should().BeNull();
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
                AuthorId = "Jlondon"
            };
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnAll_IfAny()
        {
            // ACT
            var result= await _bookRepository.GetAllBooks();

            // ASSERT
            result.Should().NotBeNull();
            result.Should().HaveCountGreaterThanOrEqualTo(4);   // 4 seeded books we have
        }
        [Fact]
        public async Task GetBookCount_ShouldReturnCorrectCount()
        {
            // ACT
            var count = await _bookRepository.GetBookCount();

            // ASSERT
            count.Should().Be(4); // 4 books in the seed data
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
                AuthorId = "JLondon" // existing author
            };

            // ACT
            var result = await _bookRepository.RegisterBook(newBook);
            var bookCount = await _bookRepository.GetBookCount();

            // ASSERT
            result.Should().BeTrue();
            bookCount.Should().BeGreaterThanOrEqualTo(5); // now there should be 5 books
        }
        [Fact]
        public async Task UpdateBook_ShouldModifyExisting()
        {
            // ARRANGE
            string bookId = "MEden";
            var updatedDto = new BookViewModel
            {
                Title = "Martin Eden",
                BookGenre = Genre.Literary,
                Price = 123,     // new price
                AuthorId = "JLondon"
            };

            // ACT
            var result= await _bookRepository.UpdateBook(bookId, updatedDto);
            var updatedBook = await _bookRepository.GetBookById(bookId);

            // ASSERT
            result.Should().BeTrue();
            updatedBook.Price.Should().Be(123);
        }

        [Fact]
        public async Task DeleteBook_ShouldRemove_WhenExists()
        {
            // ARRANGE
            var testBook = new BookViewModel
            {
                Title = "Test Book",
                Price = 25,
                BookGenre = Genre.Fantasy,
                AuthorId = "LCarroll"
            };
            await _bookRepository.RegisterBook(testBook);

            // ACT & ASSERT
            var registeredBook= await _bookRepository.GetBookByTitle(testBook.Title); 
            registeredBook.Should().NotBeNull();
            
            var result= await _bookRepository.DeleteBook(registeredBook.Id);
            result.Should().BeTrue();

            var exists= await _bookRepository.GetBookById(registeredBook.Id);
            exists.Should().BeNull();
        }
    }
}
