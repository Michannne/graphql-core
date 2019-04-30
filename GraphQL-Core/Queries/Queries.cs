using GraphQL.Types;
using GraphQLCore.Resolvers;
using System.Collections.Generic;

namespace GraphQLCore.Queries
{
    /// <summary>
    /// Interface tag for GraphQLCore queries
    /// </summary>
    public interface IGraphQLGenericQuery { }

    /// <summary>
    /// 
    /// </summary>
    internal class StitchedQuery : ObjectGraphType
    {
        /// <summary>
        /// List of resolved fields
        /// </summary>
        public List<FieldType> Fields_ { get; set; } = new List<FieldType>();

        /// <summary>
        /// Adds all resolved fields to the query
        /// </summary>
        public void Build()
        {
            foreach(var field in Fields_)
            {
                AddField(field);
            }
        }
    }

    /// <summary>
    /// A user-defined query that acts as a container until the query is supplied to a StitchedQuery instance
    /// </summary>
    /// <typeparam name="T">The return type of the query</typeparam>
    internal class GenericQuery<T> : ObjectGraphType, IGraphQLGenericQuery
    {
        /// <summary>
        /// An instance of StitchedQuery that will received the proxy query
        /// </summary>
        public StitchedQuery StitchedQuery = null;

        /// <summary>
        /// A List of queries
        /// </summary>
        public List<FieldType> Resolvers { get; set; } = new List<FieldType>();

        /// <summary>
        /// Receives a list of field resolvers to add into the container
        /// </summary>
        /// <param name="queries"></param>
        public GenericQuery(List<ITypedFieldResolver> queries = default)
        {
            if(queries != null)
                foreach (var query in queries)
                    Resolvers.Add(query.Invoke(typeof(T)));
        }

        /// <summary>
        /// Receives a field resolver to add into the container
        /// </summary>
        /// <param name="query"></param>
        public GenericQuery(ITypedFieldResolver query)
        {
            Resolvers.Add(query.Invoke(typeof(T)));
        }

        /// <summary>
        /// Combines all fields into the singleton StitchedQuery
        /// </summary>
        public void Combine()
        {
            StitchedQuery.Fields_.AddRange(Resolvers);
        }
    }
}
