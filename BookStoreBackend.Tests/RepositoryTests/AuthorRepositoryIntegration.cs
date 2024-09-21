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
    public class AuthorRepositoryIntegration: IClassFixture<IntegrationTestFixture>
    {
        private readonly AuthorRepository _authorRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthorRepositoryIntegration(IntegrationTestFixture fixture)
        {
            _context= fixture.context;
            _mapper= fixture.mapper;
            _authorRepository = new AuthorRepository(_context, _mapper);
        }
        [Fact]
        public async Task GetAuthorById_ReturnsAuthor_WhenExists()
        {
            // ARRANGE
            string authorId = "MTwain";        // an actual seeded author ID 

            // ACT
            var result = await _authorRepository.GetAuthorById(authorId);

            // ASSERT
            var successResult = result as SuccessDataResult<AuthorModel>;
            successResult.Should().NotBeNull();
            successResult.Data.FullName.Should().Be("Mark Twain");
        }

        [Fact]
        public async Task RegisterAuthor_ShouldAddAuthorToDb()
        {
            // ARRANGE
            var authorDto = new AuthorViewModel
            {
                FirstName = "Howard Phillips",
                LastName = "Lovecraft",
                Nationality = "American",
                Biography = "Was an American writer of weird, science..."
            };
            var count = await _authorRepository.GetAuthorCount();

            // ACT
            await _authorRepository.RegisterAuthor(authorDto);

            // ASSERT
            var newCount= await _authorRepository.GetAuthorCount();
            newCount.Should().Be(count + 1);
        }    

        [Fact]
        public async Task UpdateAuthor_ShouldUpdateDetails()
        {
            // ARRANGE
            var author = new AuthorModel
            {
                Id = "alDumas",
                FirstName = "Alexandre",
                LastName = "Dumas",
                Nationality = "French",
                Biography = "French writer and..."
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var updatedDto = new AuthorViewModel
            {
                FirstName = "Alexandre",
                LastName = "Dumas",
                Nationality = "French",
                Biography = "Writer of ..."
            };

            // ACT
            await _authorRepository.UpdateAuthor(author.Id, updatedDto);

            // ASSERT
            var result = await _authorRepository.GetAuthorById("alDumas");
            var successResult = result as SuccessDataResult<AuthorModel>;
            successResult.Should().NotBeNull();
            successResult.Data.Biography.Should().Be("Writer of ...");

        }
        [Fact]
        public async Task DeleteAuthor_RemovesIfExists()
        {
            // ARRANGE 
            var author = new AuthorModel
            {
                Id = "some-id",
                FirstName = "some-name",
                LastName= "some-last-name",
                Nationality = "unk",
                Biography = "unk..."
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // ACT
            await _authorRepository.DeleteAuthor(author.Id);

            // ASSERT
            var result = await _authorRepository.GetAuthorById(author.Id);
            result.IsSuccess.Should().BeFalse();
        }
    }
}
