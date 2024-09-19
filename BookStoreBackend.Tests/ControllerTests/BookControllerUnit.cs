using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using FakeItEasy;
using BookStoreBackend.Models;
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.TestUtilities;

namespace BookStoreBackend.Tests.ControllerTests
{
    public class BookControllerUnit     // testing only bussiness logic of the controller
    {
        private readonly IBookRepository _fakeBookRepo;
        private readonly ILogger<BookController> _fakeLogger;
        private readonly BookController _bookController;

        public BookControllerUnit()
        {
            _fakeBookRepo = A.Fake<IBookRepository>();
            _fakeLogger = A.Fake<ILogger<BookController>>();
            _bookController = new BookController(_fakeBookRepo, _fakeLogger);
        }

        [Fact]
        public async Task GetBookById_ReturnsBook_WhenIdExists()
        {
            // ARRANGE
            string validId = "TBProb";
            var book = new BookModel
            {
                Id = validId,
                Title = "Three Body Problem",
                BookGenre = Genre.SciFi,
                Price = 30
            };
            A.CallTo(() => _fakeBookRepo.GetBookById(validId)).Returns(Task.FromResult(book));  

            // ACT
            var result = await _bookController.GetBookById(validId);

            // ASSERT
            CommonAssertions.AssertOkDataResult<BookModel>(result, book);
        }
        [Fact]
        public async Task GetBookById_ReturnsBadReq_GivenInvalidId()
        {
            // ARRANGE
            string invalidId = "";

            // ACT
            var result = await _bookController.GetBookById(invalidId);

            // ASSERT
            CommonAssertions.AssertBadRequestResult(result);
        }

        [Fact]
        public async Task GetBookById_ReturnsNotFound_WhenNotExists()
        {
            // ARRANGE
            string notExistingId = "1";
            A.CallTo(() => _fakeBookRepo.GetBookById(notExistingId)).Returns(Task.FromResult((BookModel)null));

            // ACT
            var result = await _bookController.GetBookById(notExistingId);

            // ASSERT
            CommonAssertions.AssertNotFoundResult(result);
        }
       
        [Fact]
        public async Task GetAllBooks_ReturnsBooks_WhenAnyExists()
        {
            // ARRANGE
            var books = new List<BookModel>
            {
             new BookModel { Id = "1", Title = "Book 1",BookGenre=Genre.Fantasy,Price=22 },
             new BookModel{Id= "2", Title= "Book 2",BookGenre=Genre.Literary,Price=5},
             new BookModel{Id="3",Title="Book 3",BookGenre=Genre.Horror,Price=30},
            };
            A.CallTo(() => _fakeBookRepo.GetAllBooks())         // mocking what the repository is returning
                .Returns(Task.FromResult<IEnumerable<BookModel>>(books));

            // ACT
            var result = await _bookController.GetAllBooks();

            // ASSERT
            CommonAssertions.AssertOkDataResult<IEnumerable<BookModel>>(result, books);
        }

        [Fact]
        public async Task GetAllBooks_ReturnsNotFound_WhenNoBook()
        {
            // ARRANGE
            var booksNull= null as List<BookModel>;
            var booksEmpty = new List<BookModel>();


            // ACT & ASSERT (double)
            A.CallTo(() => _fakeBookRepo.GetAllBooks())
                .Returns(Task.FromResult<IEnumerable<BookModel>>(booksNull));
            var nullResult = await _bookController.GetAllBooks();
            CommonAssertions.AssertNotFoundResult(nullResult);

            A.CallTo(()=> _fakeBookRepo.GetAllBooks())
                .Returns(Task.FromResult<IEnumerable<BookModel>>(booksEmpty));
            var emptyResult= await _bookController.GetAllBooks();  
            CommonAssertions.AssertNotFoundResult(emptyResult);

        }

        [Fact]
        public async Task RegisterBook_ReturnsSuccess_WhenValidDto()
        {
            // ARRANGE
            var bookDto = new BookViewModel 
            {
                Title = "some-very-valid-book",
                Price= 25,
                BookGenre=Genre.Drama
            };
            A.CallTo(() => _fakeBookRepo.RegisterBook(bookDto))
                .Returns(true);

            // ACT
            var result = await _bookController.RegisterBook(bookDto);

            // ASSERT
            CommonAssertions.AssertOkResult(result);
        }
        [Fact]
        public async Task RegisterBook_ReturnsBadReq_WithModelStateErrors()
        {
            // ARRANGE (simulating invalid dto)
            _bookController.ModelState.AddModelError("Title", "Title is required");

            // ACT
            var result = await _bookController.RegisterBook(new BookViewModel());

            // ASSERT
            CommonAssertions.AssertBadRequestDataResult(result);
        }

        [Fact]
        public async Task UpdateBook_ReturnsSuccess_WhenValidDto()
        {
            // ARRANGE
            var bookDto = new BookViewModel
            {
                Title = "Updated Valid Book",
                Price = 22,
                BookGenre = Genre.Drama
            };
            A.CallTo(() => _fakeBookRepo.UpdateBook("someExistingId", bookDto)).Returns(true);

            // ACT
            var result = await _bookController.UpdateBook("someExistingId", bookDto);

            // ASSERT
            CommonAssertions.AssertOkResult(result);
        }

        [Fact]
        public async Task UpdateBook_ReturnsBadReq_WhenInvalidDto()
        {
            // ARRANGE
            _bookController.ModelState.AddModelError("Title", "Title is required");

            // ACT
            var result = await _bookController.UpdateBook("someId", new BookViewModel());

            // ASSERT
            CommonAssertions.AssertBadRequestDataResult(result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsSuccess_WhenIdExists()
        {
            // ARRANGE
            string validId = "someExistingId";
            A.CallTo(()=> _fakeBookRepo.DeleteBook(validId)).Returns(true);

            // ACT
            var result = await _bookController.DeleteBook(validId);

            // ASSERT
            CommonAssertions.AssertOkResult(result);
        }

        [Fact]
        public async Task DeleteBook_ReturnsBadRequest_WhenInvalidId()
        {
            // ARRANGE
            string invalidId = "";

            string notExistingId = "someNoneId";

            // ACT & ASSERT
            var invalidResult = await _bookController.DeleteBook(invalidId);
            CommonAssertions.AssertBadRequestResult(invalidResult);

            A.CallTo(() => _fakeBookRepo.DeleteBook(notExistingId)).Returns(false);
            var noneResult = await _bookController.DeleteBook(notExistingId);
            CommonAssertions.AssertBadRequestResult(noneResult);
        }
    }
}
