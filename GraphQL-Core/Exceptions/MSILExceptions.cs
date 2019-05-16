using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLCore.Exceptions
{
    public class GraphQLCoreMSILException : Exception
    {
        public GraphQLCoreMSILException() { }
        public GraphQLCoreMSILException(string message) : base(message) { }
        public GraphQLCoreMSILException(string message, Exception inner) : base(message, inner) { }
    }

    public class GraphQLCoreAssemblyException : Exception
    {
        public GraphQLCoreAssemblyException() { }
        public GraphQLCoreAssemblyException(string message) : base(message) { }
        public GraphQLCoreAssemblyException(string message, Exception inner) : base(message, inner) { }
    }

    public class GraphQLCoreClassDefinitionException : Exception
    {
        public GraphQLCoreClassDefinitionException() { }
        public GraphQLCoreClassDefinitionException(string message) : base(message) { }
        public GraphQLCoreClassDefinitionException(string message, Exception inner) : base(message, inner) { }
    }
}
