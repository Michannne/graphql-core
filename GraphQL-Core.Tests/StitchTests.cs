using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphQLCore;
using GraphQL_Core.Tests.Models;

namespace GraphQL_Core.Tests
{
    [TestClass]
    public class StitchTests
    {
        public Initializer initializer { get; set; } = new Initializer();

        [TestMethod]
        [TestCategory("Stitch")]
        public void Test_StitchUserModels_ParseField()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
                .Type<Book>()
                .Build();
        }
    }
}
