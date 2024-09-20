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
        public async Task RegisterAuthor_ShouldAddAuthorToDb()
        {
            // ARRANGE
            var authorDto = new AuthorViewModel
            {
                FirstName = "Socrates",
                LastName = "son of Aeschines",
                Nationality = "Greek",
                Biography = "Ancient greek thinker..."
            };
            var count = await _authorRepository.GetAuthorCount();

            // ACT
            await _authorRepository.RegisterAuthor(authorDto);

            // ASSERT
            var authors = await _authorRepository.GetAllAuthors();
            authors.Should().NotBeEmpty();
            authors.Count().Should().BeGreaterThan(count);

        }

        [Fact]
        public async Task GetAuthorById_ReturnsAuthor_WhenExists()
        {
            // ARRANGE
            var author = new AuthorModel
            {
                Id = "janeAu",
                FirstName = "Jane",
                LastName= "Austen",
                Nationality = "English",
                Biography = "Was an English novelist..."
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _authorRepository.GetAuthorById("janeAu");

            // ASSERT
            result.Should().NotBeNull();
            result.FullName.Should().Be("Jane Austen");
        }

        [Fact]
        public async Task UpdateAuthor_ShouldUpdateDetails()
        {
            // ARRANGE
            var author = new AuthorModel
            {
                Id = "alDumas",
                FullName = "Alexandre Dumas",
                Nationality = "French",
                Biography = "French writer and..."
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var updatedDto = new AuthorFullNameDto
            {
                FullName = "Alexandre Dumas",
                Nationality = "French",
                Biography = "Writer of ..."
            };

            // ACT
            await _authorRepository.UpdateAuthor("alDumas", updatedDto);

            // ASSERT
            var updatedAuthor = await _authorRepository.GetAuthorById("alDumas");
            updatedAuthor.Should().NotBeNull();
            updatedAuthor.Biography.Should().Be("Writer of ...");

        }
        [Fact]
        public async Task DeleteAuthor_RemovesIfExists()
        {
            // ARRANGE 
            var author = new AuthorModel
            {
                Id = "dummyId",
                FullName = "unk",
                Nationality = "unk",
                Biography = "unk..."
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // ACT
            await _authorRepository.DeleteAuthor("dummyId");

            // ASSERT
            var result = await _authorRepository.GetAuthorById("dummyId");
            result.Should().BeNull();
        }
    }
}
