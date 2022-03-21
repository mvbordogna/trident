using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Trident.EFCore.Json
{
    public class PolymorphicJsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

        public PolymorphicJsonValueConverter(ConverterMappingHints hints = default) : base(
            v => JsonConvert.SerializeObject(v, _jsonSerializerSettings),
            v => JsonConvert.DeserializeObject(v, _jsonSerializerSettings) as T, hints)
        { }
    }
}
