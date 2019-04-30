using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphQLCore.Extensions
{
    /// <summary>
    /// Extensions necessary when performing reflection-based logic in GraphQLCore
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Detects whether a given type, <paramref name="t"/>, implements the given interface, <paramref name="i"/>
        /// </summary>
        /// <param name="t">The implementation type</param>
        /// <param name="i">The interface type</param>
        /// <returns>True if <paramref name="t"/> implements <paramref name="i"/>, false otherwise </returns>
        internal static bool Implements(this Type t, Type i)
        {
            return t
                .GetInterfaces()
                .Contains(i);
        }

        /// <summary>
        /// Finds all Types which derive from a given base type
        /// <para>This method searches the assembly which contains the given type </para>
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <returns>A cref="List{Type}" containing all derived classes found </returns>
        internal static List<Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        /// <summary>
        /// Finds all Types which derive from a given base type
        /// <para>The search space is limited to within the given cref=Assembly </para>
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="assembly">The assembly to search within</param>
        /// <returns>A cref="List{Type}" containing all derived classes found </returns>
        internal static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    derivedType.IsAssignableFrom(t)
                    ).ToList();

        }

        /// <summary>
        /// Finds all Types which derive from the given type
        /// <para>The search space is limited to the scope of dynamically-generated GraphQLCoreTypeWrapper types</para>
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        internal static List<Type> FindAllGraphQLDerivedTypes(this Type T)
        {
            var derivedType = T;
            return Assembly.Load(GraphQLCoreTypeWrapperGenerator.asmBuilder.GetName())
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    derivedType.IsAssignableFrom(t)
                    ).ToList();

        }
    }
}
