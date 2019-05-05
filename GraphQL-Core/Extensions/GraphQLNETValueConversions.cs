using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQLCore.Extensions
{
    public static class GraphQLNETValueConversions
    {
        public static object DecimalToDouble(object d)
        {
            return Convert.ToDouble(d);
        }
    }
}
