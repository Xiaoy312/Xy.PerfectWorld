using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xy.DataAnalysis.Extension
{
    public static class ReflectionExtension
    {
        public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttributes(typeof(T), true).Any();
        }
    }
}
