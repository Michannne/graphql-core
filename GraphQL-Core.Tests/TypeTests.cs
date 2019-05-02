using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphQLCore;
using GraphQL_Core.Tests.Models;
using GraphQLCore.Types;
using GraphQLCore.Queries;
using System;
using GraphQLCore.Resolvers;
using System.Reflection;
using GraphQLCore.GraphQL;

namespace GraphQL_Core.Tests
{
    [TestClass]
    public class TypeTests
    {
        public Initializer initializer { get; set; } = new Initializer();

        [TestMethod]
        [TestCategory("Types")]
        [DataTestMethod()]
        [DataRow(typeof(Book))]
        [DataRow(typeof(Book_WithValueTypes))]
        [DataRow(typeof(Book_WithEnumTypes))]
        [DataRow(typeof(Book_WithEnumerables))]
        [DataRow(typeof(Book_WithAdvancedEnumerables))]
        [DataRow(typeof(Book_WithAuthor))]
        [DataRow(typeof(Author))]
        [DataRow(typeof(Author_WithValueTypes))]
        [DataRow(typeof(Author_WithEnumTypes))]
        [DataRow(typeof(Author_WithEnumerableBooks))]
        [DataRow(typeof(Author_WithManyManyBooks))]
        public void Test_AddType_HasDynamicallyCreatedModel(Type T)
        {
            var testMethod = this.GetType().GetMethod("Generic_Test_AddType_HasDynamicallyCreatedModel");
            testMethod.MakeGenericMethod(T).Invoke(this, null);
        }

        [TestMethod]
        [TestCategory("Types")]
        [DataTestMethod()]
        [DataRow(typeof(Book))]
        [DataRow(typeof(Book_WithValueTypes))]
        [DataRow(typeof(Book_WithEnumTypes))]
        [DataRow(typeof(Book_WithEnumerables))]
        [DataRow(typeof(Book_WithAdvancedEnumerables))]
        [DataRow(typeof(Book_WithAuthor))]
        [DataRow(typeof(Author))]
        [DataRow(typeof(Author_WithValueTypes))]
        [DataRow(typeof(Author_WithEnumTypes))]
        [DataRow(typeof(Author_WithEnumerableBooks))]
        [DataRow(typeof(Author_WithManyManyBooks))]
        public void Test_AddType_HasFields(Type T)
        {
            var testMethod = this.GetType().GetMethod("Generic_Test_AddType_HasFields");
            testMethod.MakeGenericMethod(T).Invoke(this, null);
        }

        [TestMethod]
        [TestCategory("Types")]
        [DataTestMethod()]
        [DataRow(typeof(Book))]
        [DataRow(typeof(Book_WithValueTypes))]
        [DataRow(typeof(Book_WithEnumTypes))]
        [DataRow(typeof(Book_WithEnumerables))]
        [DataRow(typeof(Book_WithAdvancedEnumerables))]
        [DataRow(typeof(Book_WithAuthor))]
        [DataRow(typeof(Author))]
        [DataRow(typeof(Author_WithValueTypes))]
        [DataRow(typeof(Author_WithEnumTypes))]
        [DataRow(typeof(Author_WithEnumerableBooks))]
        [DataRow(typeof(Author_WithManyManyBooks))]
        public void Test_AddType_IsQueryable(Type T)
        {
            var testMethod = this.GetType().GetMethod("Generic_Test_AddType_IsQueryable");
            testMethod.MakeGenericMethod(T).Invoke(this, null);
        }

        public void Generic_Test_AddType_HasFields<T>()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
                .Type<T>()
                .Build();

            var dynamicallyCreatedClass = GraphQLCoreTypeWrapperGenerator.GetDerivedGenericUserType<GenericType<T>>();

            Assert.IsNotNull(dynamicallyCreatedClass);

            var userModelInstance = initializer.resolver.Resolve(dynamicallyCreatedClass) as GenericType<T>;

            Assert.IsNotNull(userModelInstance);

            foreach (var field in typeof(T).GetProperties())
            {
                Assert.IsTrue(userModelInstance.HasField(field.Name));
            }
        }

        public void Generic_Test_AddType_HasDynamicallyCreatedModel<T>()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
                .Type<T>()
                .Build();

            var dynamicallyCreatedClass = GraphQLCoreTypeWrapperGenerator.GetDerivedGenericUserType<GenericType<T>>();

            Assert.IsNotNull(dynamicallyCreatedClass);
        }

        public void Generic_Test_AddType_IsQueryable<T>()
        {
            initializer.Init();
            var builder = initializer.services.AddGraphQL()
                .Type<T>();

            foreach (var field in typeof(T).GetProperties())
            {
                GraphQLQuery defaultQuery = 
                    () => new Query() {
                        Expression = $"get_{field.Name}",
                        Resolver = (context) => Activator.CreateInstance(field.PropertyType)
                    };

                var methodInfo = builder
                    .GetType()
                    .GetInterface("IGraphQLBuilder")
                    .GetMethod("Query");

                methodInfo
                    .MakeGenericMethod(field.PropertyType)
                    .Invoke(
                        builder, 
                        new object[] { defaultQuery });
            }

            builder.Build();

            var dynamicallyCreatedClass = GraphQLCoreTypeWrapperGenerator.GetDerivedGenericUserType<GenericType<T>>();

            Assert.IsNotNull(dynamicallyCreatedClass);

            var userModelInstance = initializer.resolver.Resolve(dynamicallyCreatedClass) as GenericType<T>;

            foreach (var field in typeof(T).GetProperties())
            {
                Assert.IsTrue(userModelInstance.HasField(field.Name));
            }
        }
    }
}
