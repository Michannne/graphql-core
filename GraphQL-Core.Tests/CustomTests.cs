using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphQLCore;
using GraphQL_Core.Tests.Models;
using GraphQLCore.Types;
using System;
using GraphQLCore.Resolvers;
using GraphQLCore.GraphQL;
using GraphQL_Core.Tests.Models.Enum;
using GraphQL.Types;
using GraphQL;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace GraphQL_Core.Tests
{
    /// <summary>
    /// To be used for customized tests
    /// Any imported models should be removed upon a PR
    /// This class should not be added to source control
    /// 
    /// Recommended to remove any additional using statements and NuGet packages downloaded for testing
    /// </summary>
    [TestClass]
    public class CustomTests
    {
        public Initializer initializer { get; set; } = new Initializer();

        [TestMethod]
        public void Configure()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
            .Build();

            initializer.Ask($@"
                
            ");
        } 
    }
}
