using AutoMapper;
using BookStoreBackend.Data;
using BookStoreBackend.Models;
using BookStoreBackend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.TestUtilities
{
    public class IntegrationTestFixture : IDisposable       // a fixture for repository integration testings
    {
        public ApplicationDbContext context { get; private set; }
        public IMapper mapper { get; private set; }
        public Seeder seeder { get; private set; }

        public IntegrationTestFixture()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())   // fresh guid for every test
                .Options;
            context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BookViewModel, BookModel>().ReverseMap();     // add mapping profiles
                cfg.CreateMap<AuthorViewModel,AuthorModel>().ReverseMap();
            });
            mapper = config.CreateMapper();

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<Seeder>();
            seeder = new Seeder(context, logger);
            SeedTestDatabase().Wait();
        }
        private async Task SeedTestDatabase() {
            await seeder.SeedDbContext();
        }
        public void Dispose()       // for releasing the resources explicitly
        {
            context.Dispose();
        }
    }
}
