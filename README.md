# BookStoreAPI_xUnitTesting

## Overview

A .NET Core Web API for a bookstore 📚 with a primary focus on testing functionality 🧪, including unit, integration, and end-to-end (E2E) tests.

## Features

- **BookStore Web API**: A basic RESTful API built with ASP.NET Core.
- **Unit Testing**: Focus on isolated functionality of API Controllers using xUnit and FluentAssertions.
- **Integration Testing**: Utilizes an in-memory database to simulate real-world interactions between repository layer and the database.
- **End-to-End (E2E) Testing**: Employed TestContainers with an MSSQL instance to replicate a full back-end system test. 

## Technologies Used

- **ASP.NET Core Web API**: Core web API framework.
- **Entity Framework Core**: Data access technology(O/RM) for the repository layer.
- **xUnit**: Test framework for unit and integration testing.
- **FluentAssertions**: Provides readable assertions to improve test clarity and debugging.
- **FakeItEasy**: Dynamic fake framework for creating all types of fake objects, mocks, stubs etc.
- **In-memory Database**: For lightweight integration testing without external dependencies.
- **TestContainers (MSSQL)**: Containers used for end-to-end testing to simulate real-world databases.
- **SwaggerUI**: For visualizing and interacting with the API’s resources/endpoints.

## Setting Up

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for E2E tests with TestContainers)
- MSSQL (for full production use, if required)
  
### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/OzturkVedat/BookStoreAPI_xUnitTesting.git
   cd BookStoreAPI_xUnitTesting
   ```

2. Restore the .NET dependencies:

    ```bash
    dotnet restore
    ```

3. Build the project:

    ```bash
    dotnet build
    ```
    
4. Set up database:
     ```bash
    dotnet ef database update
    ```
     
5. Run the API:

    ```bash
    dotnet run --project ./src/BookStoreBackend
    ```
### Running Tests

The project contains unit, integration, and E2E tests. You can run them using the .NET CLI, or more practically, you can run them on Visual Studio's test explorer.

### Project Structure

```bash
BookStoreAPI_xUnitTesting/
├── BookStoreBackend/
│       ├── Controllers/
│       ├── Data/
│       ├── Interfaces/        # Abstractions for repositories
│       ├── Middleware/        # Custom middleware for global error handling
│       ├── Migrations/        # EF Core Migration files
│       ├── Models/
│       ├── Repository/        # Repository pattern implementations
│       ├── appsettings.json/  # Configuration settings
│       └── Program.cs
├── BookStoreBackend.Tests/
│   ├── ControllerTests/
│       ├── AuthorControllerE2E.cs
│       ├── AuthorControllerUnit.cs
│       ├── BookControllerE2E.cs
│       └── BookControllerUnit.cs
│   ├── RepositoryTests/
│       ├── AuthorRepositoryIntegration.cs
│       └── BookRepositoryIntegration.cs
│   └── TestUtilities/
│       ├── CommonAssertions.cs            # Reusable assertion methods
│       ├── E2ETestBase.cs                 # Base setup for E2E tests
│       ├── E2ETestDbFactory.cs            # Factory for TestContainers
│       └── IntegrationTestFixture.cs      # Setup/teardown for integration tests
├── .gitignore
├── .BookStoreBackend.sln
└── README.md         # Project documentation

```

## Testing Strategies
All three tests use xUnit framework and FluentAssertions primarily.
  
### Unit Testing
Written to validate individual functionalities of API controller endpoints in isolation. These tests use FakeItEasy for mocking the repository responds and FluentAssertions to validate the results.

### Integration Testing
Designed to test the interactions between repository layer and the database. An in-memory database is used to simulate real world database interactions without needing an actual database.

### End-to-End (E2E) Testing
Performed to verify the entire system, from endpoints to database. TestContainers is used to spin up an MSSQL container for realistic database interaction in Docker.

## Contributing
Feel free to open issues or submit pull requests for improvements, especially around the testing framework or adding new test cases.
