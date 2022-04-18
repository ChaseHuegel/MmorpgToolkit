using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MmorpgToolkit
{
    public class SqlPropertyAttribute : Attribute { }

    public static class SqlPropertyAttributeExtensions
    {
        private static Dictionary<System.Type, List<PropertyInfo>>? cache = new Dictionary<System.Type, List<PropertyInfo>>();

        public static List<PropertyInfo> GetSqlProperties(this object obj)
        {
            if (cache.TryGetValue(obj.GetType(), out List<PropertyInfo>? list))
                return list;

            list = obj.GetType().GetProperties().Where(property => property.GetCustomAttribute<SqlPropertyAttribute>() != null).ToList();
            cache.Add(obj.GetType(), list);
            return list;
        }
    }
}
