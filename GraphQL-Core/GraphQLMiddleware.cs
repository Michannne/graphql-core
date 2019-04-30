using GraphQLCore.GraphQL;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;

namespace GraphQLCore
{
    /// <summary>
    /// Extension methods for ASP.NET Core MVC Middleware for GraphQLCore
    /// </summary>
    public static class GraphQLMiddleware
    {
        /// <summary>
        /// ASP.NET Core MVC Middleware for GraphQLCore
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns>An instance of IGraphQLBuilder that can be used to add types, queries, etc.</returns>
        public static IGraphQLBuilder AddGraphQL(this IServiceCollection services, Expression<Func<IGraphQLBuilder, IGraphQLBuilder>> config)
        {
            var builder = new GraphQLBuilder(services);

            config
                .Compile()
                (builder)
                .Build();

            return builder;
        }
    }
}
