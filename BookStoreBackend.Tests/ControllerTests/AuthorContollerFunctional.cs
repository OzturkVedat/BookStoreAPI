using Azure;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ResultModels;
using BookStoreBackend.Models.ViewModels;
using BookStoreBackend.Tests.Abstractions;
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
    public class AuthorContollerFunctional : BaseFunctionalTest
    {
        public AuthorContollerFunctional(FunctionalTestWebAppFactory factory) : base(factory)
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
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }
        [Fact]
        public async Task GetAllAuthors_ShouldReturnAll()
        {
            // ACT
            var response = await _client.GetAsync("/author/all-authors");

            // ASSERT
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<SuccessDataResult<List<AuthorModel>>>(jsonString, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase   // deserialize
            });

            apiResponse.Should().NotBeNull();
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Should().NotBeEmpty();
            apiResponse.Data.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task RegisterAuthorByFullName_ShouldReturnSuccess()
        {
            // ARRANGE
            var newAuthor = new AuthorFullNameDto
            {
                FullName = "Victor Hugo",
                Nationality = "French",
                Biography = "Famous French writer..."
            };

            // ACT
            var response = await _client.PostAsJsonAsync("/author/register-author-by-fullname", newAuthor);

            // ASSERT
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<SuccessResult>();
            result.Should().NotBeNull();
            result.Message.Should().Be("Author successfully registered.");
        }
        [Fact]
        public async Task UpdateAuthor_ShouldReturnSuccess()
        {
            // ARRANGE
            var authorId = "MTwain";
            var updatedDto = new AuthorFullNameDto
            {
                FullName = "Mark Twain",
                Nationality = "American",
                Biography = "American author known for his novels ..."
            };

            // ACT
            var response = await _client.PutAsJsonAsync($"/author/update-author/{authorId}", updatedDto);
            var updatedAuthorResponse = await _client.GetAsync($"/author/author-details/{authorId}");

            // ASSERT
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<SuccessResult>();
            result.Should().NotBeNull();

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

            deleteResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var authorNotExist = deleteResponse.Content.ReadFromJsonAsync<SuccessResult>();
            authorNotExist.Should().NotBeNull();

        }
    }
}
