using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Trident.EFCore.Json
{
    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default) : base(
            v => JsonHelper.Serialize(v),
            v => JsonHelper.Deserialize<T>(v), hints)
        { }
    }
}
