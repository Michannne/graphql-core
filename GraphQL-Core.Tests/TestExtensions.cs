using GraphQL;
using GraphQL_Core.Tests.Models.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL_Core.Tests
{
    public static class TestExtensions
    {
        public static dynamic DefaultUnknownProperty(this Type t)
        {
            dynamic defaultValue = null;

            switch (t)
            {
                case Type u when t == typeof(string):
                    defaultValue = "";
                    break;
                case Type u when t.IsEnum:
                    defaultValue = 11;
                    break;
                case Type u when t.IsInterface:
                    break;
                default:
                    defaultValue = Activator.CreateInstance(t);
                    break;
            }

            return defaultValue;
        }

        public static string ConstructSubFieldSelector(Type T, string subFieldSelector = "")
        {
            var implementedInterfaces = T.GetInterfaces().OfType<Type>().ToList();

            if (T.IsClass
                && !implementedInterfaces.Contains(typeof(IList))
                && !implementedInterfaces.Contains(typeof(IEnumerable))
                && !implementedInterfaces.Contains(typeof(IQueryable))
                && T != typeof(string)
                && T.GetProperties().Length > 0)
            {
                subFieldSelector += "{";

                foreach (var subfield in T.GetProperties())
                {
                    if (subfield.PropertyType.IsClass && subfield.PropertyType != typeof(string))
                        subFieldSelector += ConstructSubFieldSelector(subfield.PropertyType, subFieldSelector);
                    else
                        subFieldSelector += "\n" + subfield.Name.ToCamelCase();
                }

                subFieldSelector += "\n}";
            }

            return subFieldSelector;
        }
    }
}
