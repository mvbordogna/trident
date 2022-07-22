using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Trident.Extensions
{
    /// <summary>
    /// Class ObjectExtensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }
            
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
        }

        /// <summary>
        /// Converts to the T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public static T ConvertTo<T>(this string source)
        {
            var function = TypeExtensions.GetParserFunction(typeof(T));
            return (T)function(source);
        }

        /// <summary>
        /// Converts to the T type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public static object ConvertTo(this string source, Type type)
        {
            var function = TypeExtensions.GetParserFunction(type);
            return function(source);
        }

        /// <summary>
        /// Converts to .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDest">The type of the t dest.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>TDest.</returns>
        public static TDest CloneTo<T, TDest>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(TDest);
            }

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            return JsonConvert.DeserializeObject<TDest>(JsonConvert.SerializeObject(source, settings), settings);
        }


        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public static string ToJson(this object source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return string.Empty;
            }

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            return JsonConvert.SerializeObject(source, settings);
        }

        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>T.</returns>
        public static T FromJson<T>(this string source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            return JsonConvert.DeserializeObject<T>(source, settings);
        }

    }
}
