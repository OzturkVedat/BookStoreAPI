using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.Abstractions
{
    public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
    {
        protected HttpClient _client { get; init; }
        public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }
    }
}
