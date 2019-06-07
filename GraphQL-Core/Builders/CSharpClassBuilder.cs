using GraphQLCore.Exceptions;
using GraphQLCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace GraphQLCore
{
    /// <summary>
    /// Responsible for dynamically creating C# classes to wrap instances of GenericType{T}
    /// </summary>
    public static class GraphQLCoreTypeWrapperGenerator
    {
        public static AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(CreateDynamicAssemblyName(), AssemblyBuilderAccess.Run);
        private static Dictionary<Type, Type> genericTypeParentClasses = new Dictionary<Type, Type>();
        private static ModuleBuilder moduleBuilder = null;

        private static readonly string typeNameAppend = "GQLCoreTypeWrapper";
        private static readonly string assemblyNameAppend = "._Hidden_GraphQLCoreDynamicTypes";
        private static readonly string moduleName = "GraphQLCoreDynamic";

        public static Type CreateGraphQLTypeWrapper<T>()
        {
            try
            {
                if (genericTypeParentClasses.ContainsKey(typeof(GenericType<T>)))
                    return genericTypeParentClasses[typeof(GenericType<T>)];

                var typeBuilder = GetTypeBuilder(typeof(T).Name + typeNameAppend, typeof(GenericType<T>));
                typeBuilder.DefineDefaultConstructor(
                    MethodAttributes.Public
                    | MethodAttributes.SpecialName
                    | MethodAttributes.RTSpecialName);

                typeBuilder.SetParent(typeof(GenericType<T>));

                var objectTypeInfo = typeBuilder.CreateTypeInfo();
                var type = objectTypeInfo.AsType();

                genericTypeParentClasses[typeof(GenericType<T>)] = type;

                return type;
            }
            catch(Exception e)
            {
                throw new GraphQLCoreMSILException("Attempt to create new C# type failed. Refer to inner exception for details", e);
            }
        }

        public static Type GetDerivedGenericUserType<T>()
        {
            if(genericTypeParentClasses.ContainsKey(typeof(T)))
                return genericTypeParentClasses[typeof(T)];

            return null;
        }

        public static Type GetDerivedGenericUserType(this Type T)
        {
            if (genericTypeParentClasses.ContainsKey(T))
                return genericTypeParentClasses[T];

            return null;
        }

        public static Type GetBaseGenericUserType(this Type T)
        {
            if (genericTypeParentClasses.ContainsValue(T))
                return genericTypeParentClasses.Where(p => p.Value == T).FirstOrDefault().Key;

            return null;
        }

        private static AssemblyName CreateDynamicAssemblyName()
        {
            try
            {
                var currentAssemblyName = Assembly.GetEntryAssembly().GetName().Clone() as AssemblyName;
                var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
                assemblyName.Name = currentAssemblyName.Name + assemblyNameAppend;

                return currentAssemblyName;
            }
            catch(Exception e)
            {
                throw new GraphQLCoreAssemblyException("Attempt to create new dynamic assembly failed. Refer to inner exception for details", e);
            }
        }

        private static TypeBuilder GetTypeBuilder(string typeName, Type baseClass)
        {
            try
            {
                var typeSignature = typeName;

                if (moduleBuilder is null)
                    moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);

                TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                        TypeAttributes.Public |
                        TypeAttributes.Class |
                        TypeAttributes.AutoClass |
                        TypeAttributes.AnsiClass |
                        TypeAttributes.BeforeFieldInit |
                        TypeAttributes.AutoLayout,
                        baseClass);
                return tb;
            }
            catch(Exception e)
            {
                throw new GraphQLCoreClassDefinitionException("Attempt to define a new C# type failed. Refer to inner exception for details", e);
            }
        }

        public static void Clear()
        {
            genericTypeParentClasses.Clear();
            moduleBuilder = null;
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(CreateDynamicAssemblyName(), AssemblyBuilderAccess.Run);
        }
    }
}
