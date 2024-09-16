using BookStoreBackend.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace BookStoreBackend.Tests.Abstractions
{
    public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()   // spin up a disposable mssql container
            .WithPassword("Mssql-01")
            .WithImage("mcr.microsoft.com/mssql/server:latest")     // docker db image name
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(_dbContainer.GetConnectionString()));

                // Apply migrations
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();
                }

                // Seed data
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
                    seeder.SeedDbContext().Wait();  // ensure seeding completes
                }
            });
        }

        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }
        public new Task DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
