using GraphQL;
using GraphQL.Types;
using GraphQLCore.Exceptions;
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
        public static (Type, bool, Type) GetConvertibleBaseCSharpType(this Type cSharpType)
        {
            try
            {
                Type returnType = null;
                bool nullable = false;
                Type baseGraphQLType = null;


                if (cSharpType.IsEnum)
                {
                    returnType = typeof(EnumerationGraphType<>).MakeGenericType(cSharpType);
                    baseGraphQLType = returnType;
                    nullable = true;
                }

                else if (false
                    || cSharpType.IsValueType
                    || cSharpType == typeof(string))
                {
                    if (cSharpType.IsGenericType && cSharpType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        nullable = true;
                        cSharpType = cSharpType.GenericTypeArguments[0];
                    }

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
                                baseGraphQLType = returnType;
                            }
                            break;
                        case Type floaty when false
                            || cSharpType == typeof(float)
                            || cSharpType == typeof(decimal)
                            || cSharpType == typeof(double):
                            {
                                returnType = typeof(double);
                                baseGraphQLType = returnType;
                            }
                            break;
                        case Type alpha when false
                            || cSharpType == typeof(char)
                            || cSharpType == typeof(string):
                            {
                                returnType = typeof(string);
                                baseGraphQLType = returnType;
                                nullable = true;
                            }
                            break;
                        case Type boolean when false
                            || cSharpType == typeof(bool):
                            {
                                returnType = typeof(bool);
                                baseGraphQLType = returnType;
                            }
                            break;
                        case Type date when false
                            || cSharpType == typeof(DateTime):
                            {
                                returnType = typeof(DateTimeGraphType);
                                baseGraphQLType = returnType;
                                nullable = true;
                            }
                            break;
                        case Type time when false
                            || cSharpType == typeof(TimeSpan):
                            {
                                returnType = typeof(TimeSpanSecondsGraphType);
                                baseGraphQLType = returnType;
                                nullable = true;
                            }
                            break;
                        case Type timeoffset when false
                            || cSharpType == typeof(DateTimeOffset):
                            {
                                returnType = typeof(DateTimeOffsetGraphType);
                                baseGraphQLType = returnType;
                                nullable = true;
                            }
                            break;
                        case Type guid when false
                            || cSharpType == typeof(Guid):
                            {
                                returnType = typeof(GuidGraphType);
                                baseGraphQLType = returnType;
                                nullable = true;
                            }
                            break;
                        case Type str when false
                            || cSharpType.IsValueType && !cSharpType.IsPrimitive:
                            {
                                var genericType = typeof(GenericType<>).MakeGenericType(str);
                                var derived = genericType.GetDerivedGenericUserType();
                                returnType = derived;
                                baseGraphQLType = returnType;
                                nullable = true;
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
                        return (null, false, null);

                    (var underClass, _, _) = GetConvertibleBaseCSharpType(listType);
                    if (!underClass.Implements(typeof(IGraphType)))
                        underClass = underClass.GetGraphTypeFromType(true);
                    returnType = typeof(ListGraphType<>).MakeGenericType(underClass);
                    baseGraphQLType = underClass;
                    nullable = true;
                }

                else
                {
                    var genericType = typeof(GenericType<>).MakeGenericType(cSharpType);
                    var derived = genericType.GetDerivedGenericUserType();
                    baseGraphQLType = derived;
                    returnType = derived;
                    nullable = true;
                }

                return (returnType, nullable, baseGraphQLType);
            }
            catch(Exception e)
            {
                throw new Exception("Attempt to convert the type {cSharpType.Name} to it's GraphQL-equivalent failed", e);
            }
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
            (Type graphQlType, var nullable, var underClass) = GetConvertibleBaseCSharpType(cSharpType);

            if(graphQlType is null)
                throw new GraphQLCoreConversionException(
                    $"The C# type '{cSharpType.Name}' is not currently supported for auto-conversion into a GraphQL Node-type. " +
                    $"Consider registering it as a custom class in your GraphQL middleware");

            if (graphQlType.Implements(typeof(IGraphType)))
                return graphQlType;

            else
                return graphQlType.GetGraphTypeFromType(nullable);
        }

        /// <summary>
        /// Given a C# Type instance that represents a value type, enumeration, or class, returns the appropriate base GraphQL.NET Type
        /// For example, in cases such as Lists, returns the GraphQL.NET type of the generic argument 
        /// </summary>
        /// <param name="cSharpType">The C# Type to convert</param>
        /// <returns>An appropriate GraphQL.NET Type</returns>
        public static Type GetBaseGraphQLType(this Type cSharpType)
        {
            (Type graphQlType, var nullable, var underClass) = GetConvertibleBaseCSharpType(cSharpType);

            if (graphQlType is null)
                throw new GraphQLCoreConversionException(
                    $"The C# type '{cSharpType.Name}' is not currently supported for auto-conversion into a GraphQL Node-type. " +
                    $"Consider registering it as a custom class in your GraphQL middleware");

            if (underClass.Implements(typeof(IGraphType)))
                return underClass;

            else
                return underClass.GetGraphTypeFromType(nullable);
        }

        /// <summary>
        /// GraphQL.NET does not support a number of conversions
        /// </summary>
        public static void AddUnsupportedGraphQLConversions()
        {
            ValueConverter.Register(typeof(decimal), typeof(double), GraphQLNETValueConversions.DecimalToDouble);
        }

        public static bool IsExtendedGraphQLType(Type T)
        {
            return T.BaseType == typeof(EnumerationGraphType)
                    || T == typeof(GuidGraphType)
                    || T == typeof(DateTimeGraphType)
                    || T == typeof(DateTimeOffsetGraphType)
                    || T == typeof(TimeSpanSecondsGraphType)
                    || T == typeof(TimeSpanMillisecondsGraphType);
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }
}
