using GraphQL.Types;
using GraphQLCore.Extensions;
using GraphQLCore.GraphQL;
using System;

namespace GraphQLCore.Types
{
    /// <summary>
    /// Interface tag for GraphQLCore Types
    /// </summary>
    public interface IGraphQLGenericType { }

    /// <summary>
    /// Acts as a container for user-defined model fields for a GraphQL Type
    /// </summary>
    /// <typeparam name="T">User-defined model type</typeparam>
    public class GenericType<T> : ObjectGraphType<T>, IGraphQLGenericType
    {
        /// <summary>
        /// Initializes the GraphQL Type, using the provided user-defined model
        /// </summary>
        
        public GenericType()
            : this(null)
        {
        }

        public GenericType(IGraphQLBuilder builder = null)
        {
            var typedClass = typeof(T);
            var props = typedClass.GetProperties();
            foreach (var prop in props)
            {
                Type propGraphQLType = null;

                propGraphQLType = prop.PropertyType.TryConvertToGraphQLType();

                if(propGraphQLType is null || propGraphQLType.IsEnum)
                {
                    builder
                        .GetType()
                        .GetInterface("IGraphQLBuilder")
                        .GetMethod("Type")
                        .MakeGenericMethod(prop.PropertyType)
                        .Invoke(builder, null);

                    propGraphQLType = typeof(GenericType<>).MakeGenericType(prop.PropertyType);

                    if(propGraphQLType is null)
                        throw new InvalidCastException(
                            $"{prop.Name} was not automatically convertible into a GraphQL type. " +
                            $"Try explicitly adding this Type through the GraphQL-Core middleware.");
                }

                Field(propGraphQLType, prop.Name);
            }
        }

        /// <summary>
        /// "Stitches"-in a new field onto the GraphQL type, based on a join property, and a method which takes the value of the current parent and returns a field resolver
        /// </summary>
        /// <typeparam name="BResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="joinOn"></param>
        /// <param name="joinTo"></param>
        public void Stitch<BResult>(string expression, string joinOn, Func<object, Func<ResolveFieldContext<T>, BResult>> joinTo)
        {
            var stitchingType = typeof(BResult).ConvertToGraphQLType();
            var field = GetField(joinOn);

            object resolver(ResolveFieldContext<T> context)
            {
                var p = joinTo(
                    typeof(T)
                    .GetProperty(joinOn)
                    .GetValue(context.Source))(context);
                return p;
            }

            Field(stitchingType, expression, resolve: (context) => resolver(context));
        }
    }
}
