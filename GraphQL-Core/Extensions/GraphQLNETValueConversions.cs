using System;

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
