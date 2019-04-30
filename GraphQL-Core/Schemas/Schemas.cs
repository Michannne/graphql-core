using GraphQL;
using GraphQL.Types;
using GraphQLCore.Queries;

namespace GraphQLCore.Schemas
{
    /// <summary>
    /// Interface tag for GraphQLCore Schemas
    /// </summary>
    internal interface IGraphQLGenericSchema { }

    /// <summary>
    /// Schema which will be used to combine all user-provided queries
    /// </summary>
    public class SuperSchema : Schema, IGraphQLGenericSchema
    {
        /// <summary>
        /// Creates a new GraphQL Schema-node using the provided dependency resolver
        /// </summary>
        /// <param name="resolver"></param>
        public SuperSchema(IDependencyResolver resolver)
            : base(resolver)
        {
            Query = resolver.Resolve<StitchedQuery>();
        }
    }
}
