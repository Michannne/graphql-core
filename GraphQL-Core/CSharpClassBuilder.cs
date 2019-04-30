using GraphQLCore.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace GraphQLCore
{
    /// <summary>
    /// Responsible for dynamically creating C# classes to wrap instances of GenericType{T}
    /// </summary>
    public static class GraphQLCoreTypeWrapperGenerator
    {
        internal static AssemblyBuilder asmBuilder = AssemblyBuilder.DefineDynamicAssembly(CreateDynamicAssemblyName(), AssemblyBuilderAccess.Run);
        internal static ModuleBuilder mdBuilder = null;
        internal static string typeNameAppend = "GQLCoreTypeWrapper";
        internal static string asmNameAppend = "._Hidden_GraphQLCoreDynamicTypes";
        internal static string mdName = "GraphQLCoreDynamic";
        internal static Dictionary<Type, Type> genericTypeParentClasses = new Dictionary<Type, Type>();

        internal static Type CreateGraphQLTypeWrapper<T>()
        {
            if (genericTypeParentClasses.ContainsKey(typeof(GenericType<T>)))
                return genericTypeParentClasses[typeof(GenericType<T>)];
            TypeBuilder tb = GetTypeBuilder(typeof(T).Name + typeNameAppend);
            _ = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            tb.SetParent(typeof(GenericType<T>));
            TypeInfo objectTypeInfo = tb.CreateTypeInfo();
            Type type = objectTypeInfo.AsType();

            genericTypeParentClasses[typeof(GenericType<T>)] = type;

            return type;
        }

        internal static Type GetDerivedGenericUserType<T>()
        {
            if(genericTypeParentClasses.ContainsKey(typeof(T)))
                return genericTypeParentClasses[typeof(T)];

            return null;
        }

        internal static Type GetDerivedGenericUserType(this Type T)
        {
            if (genericTypeParentClasses.ContainsKey(T))
                return genericTypeParentClasses[T];

            return null;
        }

        private static AssemblyName CreateDynamicAssemblyName()
        {
            var currentAssemblyName = Assembly.GetEntryAssembly().GetName().Clone() as AssemblyName;
            currentAssemblyName.Name += asmNameAppend;
            return currentAssemblyName;
        }

        private static TypeBuilder GetTypeBuilder(string typeName)
        {
            var typeSignature = typeName;

            if(mdBuilder is null)
                mdBuilder = asmBuilder.DefineDynamicModule(mdName);

            TypeBuilder tb = mdBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);
            return tb;
        }
    }
}
