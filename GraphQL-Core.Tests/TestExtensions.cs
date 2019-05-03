using GraphQL_Core.Tests.Models.Enum;
using System;
using System.Collections.Generic;
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
    }
}
