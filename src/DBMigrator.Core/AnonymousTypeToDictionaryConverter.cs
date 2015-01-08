using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DbMigrator.Core
{
    internal static class AnonymousTypeToDictionaryConverter
    {
        public static IDictionary<string, object> Convert(object values)
        {
            return Convert<object>(values);
        }

        public static IDictionary<string, T> Convert<T>(object values)
        {
            var result = new Dictionary<string, T>();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
            {
                result.Add(descriptor.Name, (T)descriptor.GetValue(values));
            }

            return result;
        }
    }
}
