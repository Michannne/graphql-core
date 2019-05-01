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
                            returnType = typeof(int).GetGraphTypeFromType(numeric.IsNullable());
                        }
                        break;
                    case Type floaty when false
                        || cSharpType == typeof(float)
                        || cSharpType == typeof(decimal)
                        || cSharpType == typeof(double):
                        {
                            returnType = typeof(double).GetGraphTypeFromType(floaty.IsNullable());
                        }
                        break;
                    case Type alpha when false
                        || cSharpType == typeof(char)
                        || cSharpType == typeof(string):
                        {
                            returnType = typeof(string).GetGraphTypeFromType(alpha.IsNullable());
                        }
                        break;
                    case Type boolean when false
                        || cSharpType == typeof(bool):
                        {
                            returnType = typeof(bool).GetGraphTypeFromType(boolean.IsNullable());
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
                    throw new InvalidCastException("Cannot create a List of objects that do not resolve to a value type");

                var underClass = ConvertToGraphQLType(listType);
                returnType = typeof(ListGraphType<>).MakeGenericType(underClass);
            }

            else
            {
                var genericType = typeof(GenericType<>).MakeGenericType(cSharpType);
                var derived = genericType.GetDerivedGenericUserType();
                returnType = derived;
            }

            if (returnType is null)
                throw new NotSupportedException(
                    $"The C# type '{cSharpType.Name}' is not currently supported for auto-conversion into a GraphQL Node-type. " +
                    $"Consider registering it as a custom class in your GraphQL middleware");

            return returnType;
        }
    }
}
