using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Trident.EFCore.Converters
{
    public class TypeValueConverter : ValueConverter<Type, string> 
    {
        public TypeValueConverter(ConverterMappingHints hints = default) : base(
            v => $"{v.FullName}, { v.Assembly.FullName}",
            v => (!string.IsNullOrWhiteSpace(v)) ? Type.GetType(v) : null, hints)
        { }
    }
}



