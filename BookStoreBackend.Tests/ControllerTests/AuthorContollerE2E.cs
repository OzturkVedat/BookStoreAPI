using Azure;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.ControllerTests
{
    public class AuthorContollerE2E : E2ETestBase
    {
        public AuthorContollerE2E(E2ETestDbFactory factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetAuthorById_ShouldReturnBadReq_WhenInvalidId()
        {
            // ARRANGE
            string authorId = "some-not-existing-id";

            // ACT
            var response = await _client.GetAsync($"/author/author-details/{authorId}");

            // ASSERT
            await CommonAssertions.AssertHttpNotFoundResponse( response );
        }
        [Fact]
        public async Task GetAllAuthors_ShouldReturnAll()
        {
            // ACT
            var response = await _client.GetAsync("/author/all-authors");

            // ASSERT
            await CommonAssertions.AssertAndDeserializeHttpOkResponse(response);
        }

        [Fact]
        public async Task RegisterAuthor_ShouldReturnSuccess()
        {
            // ARRANGE
            var newAuthor = new AuthorViewModel
            {
                FirstName = "Victor",
                LastName = "Hugo",
                Nationality = "French",
                Biography = "Famous French writer..."
            };

            // ACT
            var response = await _client.PostAsJsonAsync("/author/register-author", newAuthor);

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response);
        }
        [Fact]
        public async Task UpdateAuthor_ShouldReturnSuccess()
        {
            // ARRANGE
            var authorId = "MTwain";
            var updatedDto = new AuthorViewModel
            {
                FirstName = "Mark",
                LastName = "Twain",
                Nationality = "American",
                Biography = "American author known for his novels ..."
            };

            // ACT
            var response = await _client.PutAsJsonAsync($"/author/update-author/{authorId}", updatedDto);
            var updatedAuthorResponse = await _client.GetAsync($"/author/author-details/{authorId}");

            // ASSERT
            await CommonAssertions.AssertHttpOkResponse(response );

            var updatedAuthor = await updatedAuthorResponse.Content.ReadFromJsonAsync<SuccessDataResult<AuthorModel>>();
            updatedAuthor.Should().NotBeNull();
            updatedAuthor.Data.Biography.Should().Be("American author known for his novels ...");
        }

        [Fact]
        public async Task DeleteAuthor_ShouldRemoveFromDb()
        {
            // ARRANGE
            var authorId = "MTwain";

            // ACT
            var authorCheck = await _client.GetAsync($"/author/author-details/{authorId}");
            var deleteResponse = await _client.DeleteAsync($"author/delete-author/{authorId}");

            // ASSERT
            authorCheck.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var authorExist = await authorCheck.Content.ReadFromJsonAsync<SuccessDataResult<AuthorModel>>();
            authorCheck.Should().NotBeNull();

            await CommonAssertions.AssertHttpOkResponse(deleteResponse );

        }
    }
}
