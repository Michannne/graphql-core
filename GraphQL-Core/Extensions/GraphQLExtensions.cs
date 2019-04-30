using GraphQL;
using GraphQL.Types;
using GraphQLCore.Types;
using System;
using System.Collections;
using System.Linq;

namespace GraphQLCore.Extensions
{
    /// <summary>
    /// Contains extension methods for GraphQLCore
    /// </summary>
    public static class GraphQLExtensions
    {
        /// <summary>
        /// Given a C# Type instance that represents a value type, enumeration, or class, returns the appropriate GraphQL.NET Type
        /// </summary>
        /// <param name="cSharpType">The C# Type to convert</param>
        /// <returns>An appropriate GraphQL.NET Type</returns>
        public static Type ConvertToGraphQLType(this Type cSharpType)
        {
            Type returnType;
            if (false
                || cSharpType.IsValueType
                || cSharpType == typeof(string))
                returnType = cSharpType.GetGraphTypeFromType(cSharpType.IsNullable());
            else if (cSharpType.IsEnum)
                returnType = typeof(EnumerationGraphType).MakeGenericType(cSharpType);
            else if (cSharpType.IsClass && (
                false
                || cSharpType.Implements(typeof(IList))
                )
                || cSharpType.IsInterface && (
                false
                || cSharpType.Implements(typeof(IQueryable))
                || cSharpType.Implements(typeof(IEnumerable))
                ))
            {
                var listType = cSharpType.GenericTypeArguments[0];
                var genericType = typeof(GenericType<>).MakeGenericType(listType);
                var derived = genericType.GetDerivedGenericUserType();
                returnType = typeof(ListGraphType<>).MakeGenericType(derived);
            }
            else
            {
                var genericType = typeof(GenericType<>).MakeGenericType(cSharpType);
                var derived = genericType.GetDerivedGenericUserType();
                returnType = derived;
            }

            return returnType;
        }
    }
}
