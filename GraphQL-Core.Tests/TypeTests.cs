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
        [DataRow(typeof(Book_WithDuplicateBaseTypes))]
        [DataRow(typeof(Book_WithTwoAuthors))]
        [DataRow(typeof(Book_WithVirtualAuthorAndId))]
        [DataRow(typeof(Book_WithDateTime))]
        [DataRow(typeof(Book_WithCommonInterface))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty_WithVirtualProperty))]
        [DataRow(typeof(Book_WithConstructor))]
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
        [DataRow(typeof(Book_WithDuplicateBaseTypes))]
        [DataRow(typeof(Book_WithTwoAuthors))]
        [DataRow(typeof(Book_WithVirtualAuthorAndId))]
        [DataRow(typeof(Book_WithDateTime))]
        [DataRow(typeof(Book_WithCommonInterface))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty_WithVirtualProperty))]
        [DataRow(typeof(Book_WithConstructor))]
        public void Test_AddType_CanAddDuplicates(Type T)
        {
            var testMethod = this.GetType().GetMethod("Generic_Test_AddType_CanAddDuplicates");
            testMethod.MakeGenericMethod(T).Invoke(this, null);
        }

        [TestMethod]
        [TestCategory("Types")]
        [DataTestMethod()]
        [DataRow(typeof(Author), typeof(Book_WithAuthor))]
        [DataRow(typeof(Author), typeof(Book_WithVirtualAuthor))]
        [DataRow(typeof(Author), typeof(Book_WithDuplicateBaseTypes))]
        public void Test_AddType_CanAddTypeAlreadyReferenced(Type T, Type K)
        {
            var testMethod = this.GetType().GetMethod("Generic_Test_AddType_CanAddTypeAlreadyReferenced");
            testMethod.MakeGenericMethod(T, K).Invoke(this, null);
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
        [DataRow(typeof(Book_WithDuplicateBaseTypes))]
        [DataRow(typeof(Book_WithTwoAuthors))]
        [DataRow(typeof(Book_WithVirtualAuthorAndId))]
        [DataRow(typeof(Book_WithDateTime))]
        [DataRow(typeof(Book_WithCommonInterface))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty_WithVirtualProperty))]
        [DataRow(typeof(Book_WithConstructor))]
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
        [DataRow(typeof(Book_WithDuplicateBaseTypes))]
        [DataRow(typeof(Book_WithTwoAuthors))]
        [DataRow(typeof(Book_WithVirtualAuthorAndId))]
        [DataRow(typeof(Book_WithDateTime))]
        [DataRow(typeof(Book_WithCommonInterface))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty))]
        [DataRow(typeof(Book_WithSameInterfaceAsProperty_WithVirtualProperty))]
        [DataRow(typeof(Book_WithConstructor))]
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

        public void Generic_Test_AddType_CanAddDuplicates<T>()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
                .Type<T>()
                .Type<T>()
                .Build();

            var dynamicallyCreatedClass = GraphQLCoreTypeWrapperGenerator.GetDerivedGenericUserType<GenericType<T>>();

            Assert.IsNotNull(dynamicallyCreatedClass);
        }

        public void Generic_Test_AddType_CanAddTypeAlreadyReferenced<T, K>()
        {
            initializer.Init();
            initializer.services.AddGraphQL()
                .Type<T>()
                .Type<K>()
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
                        Resolver = (context) => 
                            field.PropertyType.DefaultUnknownProperty()
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

                var subFields = TestExtensions.ConstructSubFieldSelector(field.PropertyType, "");

                //add sub-selection is field is class, use all props
                (var hasError, var result) = initializer.Ask($@"
                    {{
                        get_{field.Name} {subFields}
                    }}
                ");

                Assert.IsFalse(hasError);
            }
        }
    }
}
