using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BookStoreBackend.Models.ResultModels;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Azure;
using BookStoreBackend.Models;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace BookStoreBackend.Tests.TestUtilities
{
    public static class CommonAssertions
    {
        // UNIT TESTING HELPERS
        public static void AssertOkResult(IActionResult result)
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessResult>();
        }
        public static void AssertOkDataResult<T>(IActionResult result, T expectedData)
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<SuccessDataResult<T>>()
                .Which.Data.Should().BeEquivalentTo(expectedData);
        }
        public static void AssertNotFoundResult(IActionResult result) {
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().BeOfType<ErrorResult>();
        }
        public static void AssertBadRequestResult(IActionResult result)
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ErrorResult>();
        }
        public static void AssertBadRequestDataResult(IActionResult result)
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<ErrorDataResult>();
        }

        // E2E TESTING HELPERS

        public static async Task AssertHttpOkResponse(HttpResponseMessage response)
        {
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var result= await response.Content.ReadFromJsonAsync<ResultModel>();
            result.Should().NotBeNull();
        }
        public static async Task AssertAndDeserializeHttpOkResponse(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonSerializer.Deserialize<SuccessDataResult<List<AuthorModel>>>(jsonString, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase   // deserialize
            });

            apiResponse.IsSuccess.Should().BeTrue();
            apiResponse.Data.Should().NotBeNull();
            apiResponse.Data.Count.Should().BeGreaterThan(1);
        }
        public static async Task AssertHttpNotFoundResponse(HttpResponseMessage response)
        {
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
            var result = await response.Content.ReadFromJsonAsync<ErrorResult>();
            result?.Should().NotBeNull();
        }

        public static async Task AssertHttpBadRequestResponse(HttpResponseMessage response)
        {
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorResult>();
            result?.Should().NotBeNull();
        }
        
    }
}
