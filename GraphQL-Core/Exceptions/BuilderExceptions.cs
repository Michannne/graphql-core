using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLCore.Exceptions
{
    public class GraphQLCoreStitchException : Exception
    {
        public GraphQLCoreStitchException() { }
        public GraphQLCoreStitchException(string message) : base(message) { }
        public GraphQLCoreStitchException(string message, Exception inner) : base(message, inner) { }
    }

    public class GraphQLCoreTypeException : Exception
    {
        public GraphQLCoreTypeException() { }
        public GraphQLCoreTypeException(string message) : base(message) { }
        public GraphQLCoreTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
