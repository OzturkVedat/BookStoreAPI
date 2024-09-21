using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.TestUtilities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.ControllerTests
{
    public class BookControllerE2E : E2ETestBase
    {
        public BookControllerE2E(E2ETestDbFactory factory): base(factory) { }

        [Fact]
        public async Task GetBookById_ShouldReturnOk_WhenValid()
        {
            // ARRANGE
            var validBookId = "MEden";  // a seeded book ID

            // ACT
            var response = await _client.GetAsync($"/api/book/book-details/{validBookId}");

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNotFound_WhenInvalid()
        {
            // ARRANGE
            var invalidBookId = "an-unk-id";

            // ACT
            var response = await _client.GetAsync($"/api/book/book-details/{invalidBookId}");

            // ASSERT
            await CommonAssertions.AssertHttpNotFoundResponse(response);
        }
        [Fact]
        public async Task GetAllBooks_ShouldReturnOk_WhenValidPaging()
        {
            // ARRANGE
            int page = 1;
            int pageSize = 3;

            // ACT
            var response = await _client.GetAsync($"/api/book/all-books?page={page}&pageSize={pageSize}");

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsBadRequest_WhenInvalidPaging()
        {
            // ARRANGE
            var page = -1;      // invalid page number
            var pageSize = 3;

            // ACT
            var response = await _client.GetAsync($"/api/book/all-books?page={page}&pageSize={pageSize}");

            // ASSERT
            await CommonAssertions.AssertHttpBadRequestResponse(response);
        }

        [Fact]
        public async Task RegisterBook_ShouldAddSuccessfully()
        {
            // ARRANGE
            var newBook = new BookViewModel
            {
                Title = "The Mysterious Stranger",
                AuthorId = "MTwain",
                Description = "A novel by ... ",
                BookLanguage= Language.English,
                PageCount= 220,
                Publisher= "Macmillan",
                Price= 22
            };

            // ACT
            var response = await _client.PostAsJsonAsync("/api/book/register-book", newBook);

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }

        [Fact]
        public async Task RegisterBook_ReturnsBadRequest_WithInvalidInput()
        {
            // ARRANGE
            var invalidBook = new BookViewModel
            {
                Title = "The Mysterious Stranger",
                AuthorId = "MTwain",
                // missing properties
            };

            // ACT
            var response = await _client.PostAsJsonAsync("/api/book/register-book", invalidBook);

            // ASSERT
            await CommonAssertions.AssertHttpBadRequestResponse(response);
        }
        [Fact]
        public async Task UpdateBook_ShouldModify()
        {
            // ARRANGE
            var existingBookId = "MEden"; 
            var updatedBook = new BookModel
            {
                Title = "Martin Eden",
                BookGenre = Genre.Drama,
                Price = 20,
                AuthorId = "JLondon",
                Publisher = "Penguin Classics",
                PageCount = 464,
                BookLanguage = Language.English,
                Stock = 70      // dropped down the stock from 103
            };

            // ACT
            var response = await _client.PutAsJsonAsync($"/api/book/update-book/{existingBookId}", updatedBook);

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }

        [Fact]
        public async Task DeleteBook_ShouldRemoveFromDb()
        {
            // ARRANGE
            var validBookId = "TTSawyer";       // another seeded ID

            // ACT
            var response = await _client.DeleteAsync($"/api/book/delete-book/{validBookId}");

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNotFound_WithInvalidId()
        {
            // ARRANGE
            var invalidBookId = "non-existent-id";

            // ACT
            var response = await _client.DeleteAsync($"/api/book/delete-book/{invalidBookId}");

            // ASSERT
            await CommonAssertions.AssertHttpNotFoundResponse(response);    
        }


    }
}
