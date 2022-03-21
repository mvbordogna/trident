using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Trident.EFCore.Json
{
    internal static class JsonHelper
    {

        public static T Deserialize<T>(string json) where T : class
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json);
        }

        public static string Serialize<T>(T obj) where T : class
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj);
        }

    }
}
