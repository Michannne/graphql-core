using GraphQL.Types;
using GraphQLCore.Exceptions;
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
            try
            {
                var typedClass = typeof(T);
                var props = typedClass.GetProperties();
                foreach (var prop in props)
                {
                    Type propGraphQLType = null;

                    propGraphQLType = prop.PropertyType.TryConvertToGraphQLType();

                    if (propGraphQLType != null && GraphQLExtensions.IsExtendedGraphQLType(propGraphQLType))
                    {
                        var resolvedType = propGraphQLType;
                        builder
                            .GetType()
                            .GetInterface("IGraphQLBuilder")
                            .GetMethod("Type")
                            .MakeGenericMethod(resolvedType)
                            .Invoke(builder, null);
                    }
                    else if (propGraphQLType is null)
                    {
                        var resolvedType = propGraphQLType ?? prop.PropertyType;

                        builder
                            .GetType()
                            .GetInterface("IGraphQLBuilder")
                            .GetMethod("Type")
                            .MakeGenericMethod(resolvedType)
                            .Invoke(builder, null);

                        propGraphQLType = typeof(GenericType<>).MakeGenericType(resolvedType);

                        if (propGraphQLType is null)
                            throw new InvalidCastException(
                                $"{prop.Name} was not automatically convertible into a GraphQL type. " +
                                $"Try explicitly adding this Type through the GraphQL-Core middleware.");

                        propGraphQLType = GraphQLCoreTypeWrapperGenerator.GetDerivedGenericUserType(propGraphQLType);

                        if (propGraphQLType is null)
                            throw new NotSupportedException(
                                $"{prop.Name} is a custom type but was not registered through builder. " +
                                $"Try explicitly adding this Type through the Graph-Core middleware");
                    }

                    Field(propGraphQLType, prop.Name);
                }
            }
            catch(Exception e)
            {
                throw new GraphQLCoreTypeException($"An attempt to create a generic type for type {nameof(T)} failed. Refer to inner exception for details", e);
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
            try
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
            catch(Exception e)
            {
                throw new GraphQLCoreStitchException($"An error occurred while attempting to stitch a field of type {nameof(BResult)} into type {nameof(T)}", e);
            }
        }
    }
}
