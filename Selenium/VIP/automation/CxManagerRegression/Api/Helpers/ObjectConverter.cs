using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Api.Helpers
{
    /// <summary>
    /// Object-to-dictionary bidirectional converter
    /// </summary>
    internal static class ObjectConverter
    {
        private static readonly ThreadLocal<Dictionary<Type, PropertyInfo[]>> PropertiesCache =
            new ThreadLocal<Dictionary<Type, PropertyInfo[]>>();

        /// <summary>
        /// Converts dictionary with <see cref="string"/> keys and values of any types to
        /// an object of specified type with properties and their values respectively
        /// </summary>
        /// <typeparam name="T">Target object type</typeparam>
        /// <param name="source">Dictionary to be converted</param>
        /// <returns>(<see cref="T"/>) Result as object of target type</returns>
        internal static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            var someObject = new T();

            foreach (var item in source)
            {
                someObject.GetProperty(item.Key)?.SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        /// <summary>
        /// Converts object to dictionary with object property names as dictionary
        /// <see cref="string"/> keys and property values as dictionary
        /// <see cref="object"/> values
        /// </summary>
        /// <param name="source">Object to be converted</param>
        /// <returns>(<see cref="IDictionary{String, Object}"/>) Result as dictionary</returns>
        internal static IDictionary<string, object> ToDictionary(this object source)
        {
            var dictionary = new Dictionary<string, object>();

            MapToDictionaryInternal(dictionary, source);

            return dictionary;
        }

        /// <summary>
        /// Object to dictionary converter
        /// </summary>
        /// <param name="dictionary">Result dictionary</param>
        /// <param name="source">Source object</param>
        /// <param name="name">Non-primitive and non-string source object property name to use
        /// it as a key in result dictionary record.
        /// <para>CAUTION! This parameter is used for internal recursions within the method.
        /// DO NOT USE in external method calls.</para></param>
        private static void MapToDictionaryInternal(
            IDictionary<string, object> dictionary, object source, string name = "")
        {
            var properties = source.GetProperties();
            
            // to process primitive/string values in collections in recursive calls
            if (properties.Length == 0 && !string.IsNullOrEmpty(name))
            {
                var type = source.GetType();
                if (type.IsPrimitive || type == typeof(string))
                {
                    dictionary[name] = source;
                    return;
                }
            }

            foreach (var property in properties)
            {
                var key = string.IsNullOrEmpty(name) ? property.Name : name + $"[{property.Name}]";
                var value = property.GetValue(source, null);
                if (value == null)
                {
                    continue;
                }
                var valueType = value.GetType();

                if (valueType.IsPrimitive || valueType == typeof(string))
                {
                    dictionary[key] = value;
                }
                else 
                {
                    if (value is IEnumerable enumerable)
                    {
                        var j = 0;
                        foreach (var obj in enumerable)
                        {
                            MapToDictionaryInternal(dictionary, obj, $"{key}[{j}]");
                            j++;
                        }
                    }
                    else
                    {
                        MapToDictionaryInternal(dictionary, value, key);
                    }
                }
            }
        }

        /// <summary>
        /// Gets type properties from cache
        /// </summary>
        /// <param name="source">Source object</param>
        /// <returns>An array of <see cref="PropertyInfo" /> objects representing all
        /// public properties of the current <see cref="Type" />. -or- An empty array of type
        /// <see cref="PropertyInfo" />, if the current <see cref="Type" />
        /// does not have public properties.</returns>
        private static PropertyInfo[] GetProperties(this object source)
        {
            var type = source.GetType();
            if (!PropertiesCache.IsValueCreated)
            {
                PropertiesCache.Value = new Dictionary<Type, PropertyInfo[]>();
            }

            if (!PropertiesCache.Value.ContainsKey(type))
            {
                PropertiesCache.Value.Add(type, type.GetProperties());
            }

            return PropertiesCache.Value.SingleOrDefault(x => x.Key == type).Value;
        }

        /// <summary>
        /// Requests type properties cache. Returns a property by name.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="name">Source object's property name</param>
        /// <returns><see cref="PropertyInfo" /> object representing the public property
        /// of the current <see cref="Type" />. -or- Null if does not have this public
        /// property.</returns>
        private static PropertyInfo GetProperty(this object source, string name)
        {
            return GetProperties(source)?.FirstOrDefault(x => x.Name == name);
        }
    }
}
