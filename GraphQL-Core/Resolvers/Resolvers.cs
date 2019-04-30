using GraphQL.Types;
using GraphQLCore.Extensions;
using System;

namespace GraphQLCore.Resolvers
{
    /// <summary>
    /// Interface for a generic field resolver
    /// </summary>
    public interface IGenericFieldResolver
    {
        /// <summary>
        /// When called, returns an instance of FieldType that can be added to any ObjectGraphType
        /// </summary>
        /// <returns></returns>
        FieldType Invoke();
    }

    /// <summary>
    /// A typed-field resolver, which receives a type before returning a FieldType
    /// </summary>
    public interface ITypedFieldResolver
    {
        FieldType Invoke(Type type);
    }

    /// <summary>
    /// An alias for a generic field resolver
    /// </summary>
    public class Query : GenericFieldResolver
    { }

    /// <summary>
    /// 
    /// </summary>
    public class GenericFieldResolver : ObjectGraphType, ITypedFieldResolver
    {
        public string Expression { get; set; }
        public new string Description { get; set; }
        public new string DeprecationReason { get; set; }
        public QueryArguments Args { get; set; }
        public Func<ResolveFieldContext<object>, object> Resolver { get; set; }

        public FieldType Invoke(Type cSharpType)
        {
            return Field(cSharpType.ConvertToGraphQLType(), Expression, Description, Args, Resolver, DeprecationReason);
        }
    }
}
