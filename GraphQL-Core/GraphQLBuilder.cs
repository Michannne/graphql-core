using GraphQL;
using GraphQL.Types;
using GraphQLCore.Queries;
using GraphQLCore.Schemas;
using GraphQLCore.Resolvers;
using GraphQLCore.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQLCore.Extensions;

namespace GraphQLCore.GraphQL
{
    /// <summary>
    /// The type signature for GraphQLCore queries
    /// </summary>
    /// <returns></returns>
    public delegate ITypedFieldResolver GraphQLQuery();

    /// <summary>
    /// An interface that can be used to provide builder configurations for GraphQLBuilder
    /// </summary>
    public interface IGraphQLConfiguration
    {
        /// <summary>
        /// Method which can be called to configure the builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        IGraphQLBuilder Configure(IGraphQLBuilder builder);
    }

    /// <summary>
    /// Builder interface for GraphQL-Core
    /// </summary>
    public interface IGraphQLBuilder
    {
        /// <value>Stores GraphQL Query instances</value>
        List<IGraphQLGenericQuery> Queries { get; set; }

        /// <value>Stores GraphQL Type instances and their corresponding object types</value>
        Dictionary<Type, object> GraphQLTypes { get; set; }

        /// <value>Stores GraphQL Mutator instances and their corresponding object types</value>
        Dictionary<Type, object> Mutators { get; set; }

        /// <summary>
        /// Adds a new virtual GraphQL Type-node based on the corresponding model definition to the object graph
        /// </summary>
        /// <typeparam name="T">The model class type</typeparam>
        /// <returns>An instance of this GraphQBuilder</returns>
        IGraphQLBuilder Type<T>();

        /// <summary>
        /// <para>"Stitches"-in a new field onto the provided type, accessible through the provided expression, and resolved through the given resolver</para>
        /// <remarks>
        /// This stitching functionality is much slower than any native join, such as Entity Framework's .Include(), which
        /// injects JOIN statements directly in the SQL query.
        /// This method is intended to be used when a user-defined type does not implicitly reference another user-defined type
        /// </remarks>
        /// </summary>
        /// <typeparam name="A">The type which will be stitched onto</typeparam>
        /// <typeparam name="BResult">The result returned when the new field is resolved</typeparam>
        /// <param name="expr">The expression used to query the new field</param>
        /// <param name="joinOn">The property of which the stitching will be based on</param>
        /// <param name="joinTo">A method which takes an IDataUow and returns a method which takes the value of the current parent element, and returns a context resolver</param>
        /// <returns></returns>
        IGraphQLBuilder Stitch<A, BResult>(
            string expr, 
            Expression<Func<A, object>> joinOn, 
            Func< 
                Func<object, 
                    Func<
                        ResolveFieldContext<A>, 
                        BResult>>> joinTo);

        /// <summary>
        /// <para>Adds a new virtual GraphQL Query-node which returns an object of the supplied C# type once resolved</para>
        /// <para>This method will resolve any interface-based dependencies supplied to the query method</para>
        /// </summary>
        /// <typeparam name="T">The C# return type of the query</typeparam>
        /// <param name="query">The method called when resolving this query</param>
        /// <returns></returns>
        IGraphQLBuilder Query<T>(GraphQLQuery query);

        /// <summary>
        /// Adds a new virtual GraphQL Mutator-node which returns an object of the supplied C# type
        /// This method will resolve any interface-based dependencies supplied to the query method
        /// </summary>
        /// <typeparam name="T">The C# return type of the query</typeparam>
        /// <returns></returns>
        IGraphQLBuilder Mutator<T>();

        /// <summary>
        /// Creates the schema using the the builder parameters and loads it into the IServiceCollection instance
        /// </summary>
        void Build();
    }

    /// <summary>
    /// Concrete implementation of the IGraphQLBuilder interface
    /// </summary>
    public class GraphQLBuilder : IGraphQLBuilder
    {
        /// <summary>
        /// Default constructed to an empty list
        /// </summary>
        List<IGraphQLGenericQuery> IGraphQLBuilder.Queries { get; set; } = new List<IGraphQLGenericQuery>();

        /// <summary>
        /// Default constructed to an empty list
        /// </summary>
        Dictionary<Type, object> IGraphQLBuilder.GraphQLTypes { get; set; } = new Dictionary<Type, object>();

        /// <summary>
        /// Default constructed to an empty list
        /// </summary>
        Dictionary<Type, object> IGraphQLBuilder.Mutators { get; set; } = new Dictionary<Type, object>();

        /// <summary>
        /// Defaulted to null
        /// </summary>
        internal StitchedQuery Schema { get; set; }

        /// <summary>
        /// Defaulted to null
        /// </summary>
        public IDependencyResolver Resolver { get; set; }

        /// <summary>
        /// Defaulted to null
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// Creates a new GraphQLBuilder
        /// </summary>
        /// <param name="services">The service collection which will contain the added singletons</param>
        public GraphQLBuilder(IServiceCollection services)
        {
            var schema = new StitchedQuery();
            var serviceProvider = services.BuildServiceProvider();
            var resolver = new FuncDependencyResolver(type =>
            {
                var service = services.Where(svc => svc.ServiceType == type).FirstOrDefault();
                if (service is null || service.ImplementationInstance is null)
                {
                    return serviceProvider.GetService(type);
                }

                return service.ImplementationInstance;
            });
            Schema = schema;
            Resolver = resolver;
            Services = services;
            services.AddSingleton(schema);
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
        }

        IGraphQLBuilder IGraphQLBuilder.Type<T>()
        {
            var userType = GraphQLCoreTypeWrapperGenerator.CreateGraphQLTypeWrapper<T>();
            var graphQlTypeInstance = new GenericType<T>
            {
                Name = typeof(T).Name
            };
            ((IGraphQLBuilder)this).GraphQLTypes.Add(userType, graphQlTypeInstance);
            return this;
        }

        IGraphQLBuilder IGraphQLBuilder.Stitch<A, BResult>(
            string expr,
            Expression<Func<A, object>> joinOn,
            Func<
                Func<object,
                    Func<
                        ResolveFieldContext<A>,
                        BResult>>> joinTo
            )
        {
            var userTypeA = GraphQLCoreTypeWrapperGenerator.CreateGraphQLTypeWrapper<A>();
            var userTypeB = typeof(BResult).ConvertToGraphQLType();
            GenericType<A> graphQlTypeAInstance;
            if (((IGraphQLBuilder)this).GraphQLTypes.ContainsKey(userTypeA))
                graphQlTypeAInstance = (GenericType<A>)((IGraphQLBuilder)this).GraphQLTypes[userTypeA];
            else
            {
                graphQlTypeAInstance = new GenericType<A>
                {
                    Name = typeof(A).Name
                };
                ((IGraphQLBuilder)this).GraphQLTypes.Add(userTypeA, graphQlTypeAInstance);
            }

            var propertyName = (joinOn.Body as MemberExpression ?? ((UnaryExpression)joinOn.Body).Operand as MemberExpression).Member.Name;

            graphQlTypeAInstance.Stitch(expr, propertyName, joinTo());

            return this;
        }

        IGraphQLBuilder IGraphQLBuilder.Query<T>(GraphQLQuery query)
        {
            var method = query;

            var graphQlQueryInstance = new GenericQuery<T>(method());
            ((IGraphQLBuilder)this).Queries.Add(graphQlQueryInstance);
            return this;
        }

        IGraphQLBuilder IGraphQLBuilder.Mutator<T>()
        {
            ((IGraphQLBuilder)this).Mutators.Add(typeof(T), new object());
            return this;
        }

        void IGraphQLBuilder.Build()
        {
            foreach (var type in ((IGraphQLBuilder)this).GraphQLTypes)
            {
                var instance = type.Value;
                var graphQlType = type.Key;
                Services.AddSingleton(graphQlType, instance);
            }

            foreach (var query in ((IGraphQLBuilder)this).Queries)
            {
                query
                    .GetType()
                    .GetField("StitchedQuery")
                    .SetValue(query, Schema);
                query
                    .GetType()
                    .InvokeMember("Combine", BindingFlags.InvokeMethod, null, query, null);
            }

            Schema.Build();
            var superSchema = (ISchema)Activator.CreateInstance(typeof(SuperSchema), new object[] { Resolver });
            Services.AddSingleton(superSchema);
        }
    }
}
