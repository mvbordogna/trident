//using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
//using Newtonsoft.Json.Linq;

//namespace Trident.EFCore
//{
//    public class GenericEnumValueConverter<TEnum> : ValueConverter<TEnum, JToken>
//    {
//        public GenericEnumValueConverter(string propertyName, ConverterMappingHints mappingHints = null)
//            : base( 
//                  v => v.ToString(),
//                  v => v.Value<TEnum>(propertyName),               
                
                
//                  mappingHints)
//        {
//        }

//        public static ValueConverterInfo DefaultInfo { get; }
//            = new ValueConverterInfo(typeof(TEnum), typeof(JToken), i => new GenericEnumValueConverter<TEnum>("UnknownName", i.MappingHints));
//    }
//}