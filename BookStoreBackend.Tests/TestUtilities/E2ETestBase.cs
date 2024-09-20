using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Tests.TestUtilities
{
    public class E2ETestBase : IClassFixture<E2ETestDbFactory>   // utilize the testcontainer as a fixture
    {
        protected HttpClient _client { get; init; }
        public E2ETestBase(E2ETestDbFactory factory)
        {
            _client = factory.CreateClient();
        }
    }
}
