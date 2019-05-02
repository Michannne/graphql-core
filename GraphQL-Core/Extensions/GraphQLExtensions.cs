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
        public static Type GetConvertibleBaseCSharpType(this Type cSharpType)
        {
            Type returnType = null;

            if (cSharpType.IsEnum)
                returnType = typeof(EnumerationGraphType<>).MakeGenericType(cSharpType);

            else if (false
                || cSharpType.IsValueType
                || cSharpType == typeof(string))
            {
                switch (cSharpType)
                {
                    case Type numeric when false
                        || cSharpType == typeof(int)
                        || cSharpType == typeof(uint)
                        || cSharpType == typeof(long)
                        || cSharpType == typeof(ulong)
                        || cSharpType == typeof(byte)
                        || cSharpType == typeof(sbyte)
                        || cSharpType == typeof(short)
                        || cSharpType == typeof(ushort):
                        {
                            returnType = typeof(int);
                        }
                        break;
                    case Type floaty when false
                        || cSharpType == typeof(float)
                        || cSharpType == typeof(decimal)
                        || cSharpType == typeof(double):
                        {
                            returnType = typeof(double);
                        }
                        break;
                    case Type alpha when false
                        || cSharpType == typeof(char)
                        || cSharpType == typeof(string):
                        {
                            returnType = typeof(string);
                        }
                        break;
                    case Type boolean when false
                        || cSharpType == typeof(bool):
                        {
                            returnType = typeof(bool);
                        }
                        break;
                    case Type guid when false
                        || cSharpType == typeof(Guid):
                        {
                            returnType = typeof(GuidGraphType);
                        }
                        break;
                    case Type str when false
                        || cSharpType.IsValueType && !cSharpType.IsPrimitive:
                        {
                            var genericType = typeof(GenericType<>).MakeGenericType(str);
                            var derived = genericType.GetDerivedGenericUserType();
                            returnType = derived;
                        }
                        break;
                    default:
                        break;
                }
            }

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

                if (listType is null)
                    return null;

                var underClass = GetConvertibleBaseCSharpType(listType);
                if(!underClass.Implements(typeof(IGraphType)))
                    underClass = underClass.GetGraphTypeFromType();
                returnType = typeof(ListGraphType<>).MakeGenericType(underClass);
            }

            else
            {
                var genericType = typeof(GenericType<>).MakeGenericType(cSharpType);
                var derived = genericType.GetDerivedGenericUserType();
                returnType = derived;
            }

            return returnType;
        }

        public static Type TryConvertToGraphQLType(this Type cSharpType)
        {
            try
            {
                var type = cSharpType.ConvertToGraphQLType();
                return type;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Given a C# Type instance that represents a value type, enumeration, or class, returns the appropriate GraphQL.NET Type
        /// </summary>
        /// <param name="cSharpType">The C# Type to convert</param>
        /// <returns>An appropriate GraphQL.NET Type</returns>
        public static Type ConvertToGraphQLType(this Type cSharpType)
        {
            Type graphQlType = GetConvertibleBaseCSharpType(cSharpType);

            if(graphQlType is null)
                throw new NotSupportedException(
                    $"The C# type '{cSharpType.Name}' is not currently supported for auto-conversion into a GraphQL Node-type. " +
                    $"Consider registering it as a custom class in your GraphQL middleware");

            if (graphQlType.Implements(typeof(IGraphType)))
                return graphQlType;

            else
                return graphQlType.GetGraphTypeFromType(cSharpType.IsNullable());
        }
    }
}
