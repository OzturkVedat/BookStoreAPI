using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.TestUtilities;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using Xunit;
using FluentAssertions;
using AutoMapper;

namespace BookStoreBackend.Tests.ControllerTests
{
    public class AuthorControllerUnit
    {
        private readonly IAuthorRepository _fakeAuthorRepo;
        private readonly ILogger<AuthorController> _fakeLogger;
        private readonly AuthorController _authorController;
        public AuthorControllerUnit()      // constructor for unit test
        {
            _fakeAuthorRepo = A.Fake<IAuthorRepository>();      // mocking (or faking)
            _fakeLogger = A.Fake<ILogger<AuthorController>>();
            _authorController = new AuthorController(_fakeLogger, _fakeAuthorRepo);
        }
        [Fact]
        public async Task GetAuthorById_ReturnsAuthor_WhenExists()
        {
            // ARRANGE
            var authorId = "2";
            var expectedAuthor = new AuthorModel
            {
                Id = "2",
                FirstName = "Stephen",
                LastName = "King",
                Nationality = "American",
                Biography = "American author.."
            };
            A.CallTo(() => _fakeAuthorRepo.GetAuthorById(authorId))
                .Returns(new SuccessDataResult<AuthorModel>("Author details retrieved successfully.", expectedAuthor));

            // ACT
            var result = await _authorController.GetAuthorById(authorId);

            // ASSERT
            CommonAssertions.AssertOkDataResult<AuthorModel>(result, expectedAuthor);

        }

        [Fact]
        public async Task GetAuthorById_ReturnsBadRequest_WhenInvalidId()
        {
            // ARRANGE
            string invalidId = "";

            //ACT
            var result = await _authorController.GetAuthorById(invalidId);

            // ASSERT
            CommonAssertions.AssertBadRequestResult(result);

        }

        [Fact]
        public async Task GetAuthorById_ReturnsNotFound_WhenNotExist()
        {
            // ARRANGE
            var authorId = "id-not-exist";
            A.CallTo(() => _fakeAuthorRepo
            .GetAuthorById(authorId)).Returns(new ErrorResult($"No author found with ID: {authorId}"));

            // ACT
            var result = await _authorController.GetAuthorById(authorId);

            // ASSERT
            CommonAssertions.AssertNotFoundResult(result);
        }

        [Fact]
        public async Task GetAllAuthors_ReturnsAuthors_WhenExist()
        {
            // ARRANGE
            var authors = new List<AuthorModel>
                {
                    new AuthorModel { Id= "1", FirstName= "Leo",LastName= "Tolstoy", Nationality= "Russia", Biography= "Russian author.." },
                    new AuthorModel { Id= "2", FirstName = "Stephen",LastName="King", Nationality = "American", Biography = "American author.." },
                    new AuthorModel { Id= "3", FirstName= "William",LastName="Shakespeare", Nationality= "British", Biography= "British poetry.."}
                };
            A.CallTo(() => _fakeAuthorRepo.GetAllAuthors(1, 3))
                .Returns(new SuccessDataResult<IEnumerable<AuthorModel>>("Successfully fetched the requested authors.", authors));

            // ACT
            var result = await _authorController.GetAllAuthors(1,3 );   // first page, fetch three authors

            // ASSERT
            CommonAssertions.AssertOkDataResult<IEnumerable<AuthorModel>>(result, authors);
        }

        [Fact]
        public async Task GetAllAuthors_ReturnsNotFound_WhenNone()
        {
            // ARRANGE
            var authorsEmpty = new List<AuthorModel>();
            var authorsNull = null as List<AuthorModel>;

            // ACT & ASSERT
            A.CallTo(() => _fakeAuthorRepo.GetAllAuthors(1, 10))
                .Returns(new ErrorResult("No authors found for the requested page."));
            var resultEmpty = await _authorController.GetAllAuthors(1, 10);
            CommonAssertions.AssertNotFoundResult(resultEmpty);

        }

        [Fact]
        public async Task RegisterAuthor_ReturnsSuccess_WhenValidDto()
        {
            // ARRANGE
            var authorDto = new AuthorViewModel
            {
                FirstName = "Aldous",
                LastName = "Huxley",
                Nationality = "English",
                Biography = "English writer and philosopher..."
            };
            A.CallTo(() => _fakeAuthorRepo.RegisterAuthor(authorDto))
                .Returns(new SuccessResult("Author registered successfully."));

            // ACT
            var fnResult = await _authorController.RegisterAuthor(authorDto);

            // ASSERT
            CommonAssertions.AssertOkResult(fnResult);
        }
        [Fact]
        public async Task RegisterAuthor_ReturnsBadRequest_WhenInvalidDto()
        {
            // ARRANGE
            _authorController.ModelState.AddModelError("FirstName", "First name is required.");

            // ACT
            var result = await _authorController.RegisterAuthor(new AuthorViewModel());

            // ASSERT
            CommonAssertions.AssertBadRequestDataResult(result);
        }
        [Fact]
        public async Task RegisterAuthor_ReturnsBadRequest_WhenDuplicate()
        {
            // ARRANGE
            var dto = new AuthorViewModel
            {
                FirstName = "Albert",
                LastName = "Camus",
                Biography = "French philosopher and author...",
                Nationality = "French"
            };
            string Fullname = $"{dto.FirstName} {dto.LastName}";
            A.CallTo(() => _fakeAuthorRepo.RegisterAuthor(dto))
                .Returns(new ErrorResult($"Author with name {Fullname} is already registered."));

            // ACT
            var result= await _authorController.RegisterAuthor(dto);

            // ASSERT
            CommonAssertions.AssertBadRequestResult(result);

        }

        [Fact]
        public async Task UpdateAuthor_ReturnsSuccess_WhenValidData()
        {
            // ARRANGE
            var authorId = "1";
            var updatedDto = new AuthorViewModel
            {
                FirstName = "Alexander",
                LastName = "Pushkin",
                Nationality = "Russia",
                Biography = "Writer and ..."
            };
            A.CallTo(() => _fakeAuthorRepo.UpdateAuthor(authorId, updatedDto))
                .Returns(new SuccessResult("Author details updated successfully."));

            // ACT
            var result = await _authorController.UpdateAuthor(authorId, updatedDto);

            // ASSERT
            CommonAssertions.AssertOkResult(result);
        }
        [Fact]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenInvalidDto()
        {
            // ARRANGE
            _authorController.ModelState.AddModelError("Biography", "Biography cannot exceed 200 characters.");

            // ACT
            var result = await _authorController.UpdateAuthor("some-id", new AuthorViewModel());

            // ASSERT
            CommonAssertions.AssertBadRequestDataResult(result);
        }
        [Fact]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenFail()
        {
            // ARRANGE
            var authorId = "1";
            var updatedDto = new AuthorViewModel
            {
                FirstName = "Alexander",
                LastName = "Pushkin",
                Nationality = "Russia",
                Biography = "Writer and ..."
            };
            A.CallTo(() => _fakeAuthorRepo.UpdateAuthor(authorId, updatedDto))
                .Returns(new ErrorResult("Failed to update the author details."));

            // ACT
            var result = await _authorController.UpdateAuthor(authorId, updatedDto);

            // ASSERT
            CommonAssertions.AssertBadRequestResult(result);
        }

        [Fact]
        public async Task DeleteAuthor_ShouldReturnOk_WhenRemoved()
        {
            // ARRANGE
            var authorId = "3";
            A.CallTo(() => _fakeAuthorRepo.DeleteAuthor(authorId)).Returns(new SuccessResult("Author removed successfully."));

            // ACT
            var result = await _authorController.DeleteAuthor(authorId);

            // ASSERT
            CommonAssertions.AssertOkResult(result);
        }
        [Fact]
        public async Task DeleteAuthor_ShouldReturnOk_WhenNotFound()
        {
            // ARRANGE
            var authorId = "none-existing-id";
            A.CallTo(() => _fakeAuthorRepo.DeleteAuthor(authorId)).Returns(new ErrorResult($"No author found with ID: {authorId}"));

            // ACT
            var result= await _authorController.DeleteAuthor(authorId);

            // ASSERT
            CommonAssertions.AssertNotFoundResult(result);
        }
    }
}
