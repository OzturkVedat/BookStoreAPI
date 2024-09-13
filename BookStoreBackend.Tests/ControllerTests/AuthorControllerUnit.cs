using FakeItEasy;
using Xunit;
using FluentAssertions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using BookStoreBackend.Models;
using Microsoft.AspNetCore.Mvc;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;

namespace BookStoreBackend.Tests.ControllerTests
{
    public class AuthorControllerUnit
    {
        private readonly IAuthorRepository _fakeAuthorRepo;
        private readonly ILogger<AuthorController> _logger;
        private readonly AuthorController _authorController;
        public AuthorControllerUnit()      // constructor for unit test
        {
            _fakeAuthorRepo = A.Fake<IAuthorRepository>();      // mocking (or faking)
            _logger = A.Fake<ILogger<AuthorController>>();
            _authorController = new AuthorController(_logger, _fakeAuthorRepo);
        }
        [Fact]
        public async Task GetAuthorById_ReturnsOk()
        {
            // ARRANGE
            var authorId = "2";
            var author = new AuthorModel
            {
                Id = "2",
                FullName = "Stephen King",
                Nationality = "American",
                Biography = "American author.."
            };
            A.CallTo(() => _fakeAuthorRepo.GetAuthorById(authorId)).Returns(Task.FromResult(author));

            // ACT
            var result = await _authorController.GetAuthorById(authorId);

            // ASSERT
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessDataResult<AuthorModel>>();

            var authorResult = (result as OkObjectResult).Value as SuccessDataResult<AuthorModel>;
            authorResult.Data.Should().BeEquivalentTo(author);

        }

        [Fact]
        public async Task GetAuthorById_ReturnsNotFound_WhenNotExist()
        {
            // ARRANGE
            var authorId = "id-not-exist";
            A.CallTo(() => _fakeAuthorRepo
            .GetAuthorById(authorId)).Returns(Task.FromResult<AuthorModel>(null));

            // ACT
            var result = await _authorController.GetAuthorById(authorId);

            // ASSERT
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeOfType<ErrorResult>()
                .Which.Message.Should().Be("Author not found.");
        }

        [Fact]
        public async Task GetAllAuthors_ShouldReturnOk_WhenExist()
        {
            // ARRANGE
            var authors = new List<AuthorModel>
                {
                    new AuthorModel { Id= "1", FullName= "Leo Tolstoy", Nationality= "Russia", Biography= "Russian author.." },
                    new AuthorModel { Id= "2", FullName = "Stephen King", Nationality = "American", Biography = "American author.." },
                    new AuthorModel { Id= "3", FullName= "William Shakespeare", Nationality= "British", Biography= "British poetry.."}
                };
            A.CallTo(() => _fakeAuthorRepo.GetAllAuthors())
                .Returns(Task.FromResult<IEnumerable<AuthorModel>>(authors));

            // ACT
            var result = await _authorController.GetAllAuthors();

            // ASSERT
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessDataResult<IEnumerable<AuthorModel>>>()
                .Which.Data.Should().Contain(authors);
            (result as OkObjectResult).Value.Should().BeOfType<SuccessDataResult<IEnumerable<AuthorModel>>>()
                .Which.Message.Should().Be("Successfully fetched all authors.");
        }

        [Fact]
        public async Task RegisterAuthor_ShouldReturnOk_WhenValidData()
        {
            // ARRANGE
            var authorFnDto = new AuthorFullNameDto
            {
                FullName = "Aldous Huxley",
                Nationality = "English",
                Biography = "English writer and philospher..."
            };

            var authorViewModel = new AuthorViewModel
            {
                FirstName = "George R.R.",
                LastName = "Martin",
                Nationality = "American",
                Biography = "American novel writer..."
            };

            // ACT
            var fnResult = await _authorController.RegisterAuthorByFullName(authorFnDto);
            var vmResult = await _authorController.RegisterAuthor(authorViewModel);

            // ASSERT
            fnResult.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeOfType<SuccessResult>();

            vmResult.Should().BeOfType<OkObjectResult>()
              .Which.Value.Should().BeOfType<SuccessResult>();
        }
        [Fact]
        public async Task UpdateAuthor_ShouldReturnOk_WhenValidData()
        {
            // ARRANGE
            var authorId = "1";
            var updatedDto = new AuthorFullNameDto
            {
                FullName = "Alexander Pushkin",
                Nationality = "Russia",
                Biography = "Writer and ..."
            };
            A.CallTo(() => _fakeAuthorRepo.UpdateAuthor(authorId, updatedDto)).Returns(Task.CompletedTask);

            // ACT
            var result = await _authorController.UpdateAuthor(authorId, updatedDto);

            // ASSERT
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessResult>();
        }
        [Fact]
        public async Task DeleteAuthor_ShouldReturnOk()
        {
            // ARRANGE
            var authorId = "3";
            A.CallTo(() => _fakeAuthorRepo.DeleteAuthor(authorId)).Returns(Task.CompletedTask);

            // ACT
            var result = await _authorController.DeleteAuthor(authorId);

            // ASSERT
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessResult>();

        }
    }
}
