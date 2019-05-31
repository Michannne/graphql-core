using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLCore.Exceptions
{
    public class GraphQLCoreConversionException : Exception
    {
        public GraphQLCoreConversionException() { }
        public GraphQLCoreConversionException(string message) : base(message) { }
        public GraphQLCoreConversionException(string message, Exception inner) : base(message, inner) { }
    }
}
