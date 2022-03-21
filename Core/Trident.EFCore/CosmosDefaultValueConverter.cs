using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Trident.EFCore.Converters;
using Trident.EFCore.Json;

namespace Trident.EFCore
{

    public static class CosmosDefaultConverters
    {
        public static Dictionary<Type, ValueConverter> Converters = new Dictionary<Type, ValueConverter>()
        {
            { typeof(bool), new CosmosDefaultValueConverter<bool, bool?>(BoolToDb(), BoolFromDb()) },
            { typeof(char), new CosmosDefaultValueConverter<char, char?>(CharToDb(), CharFromDb()) },
            { typeof(byte), new CosmosDefaultValueConverter<byte, byte?>(ByteToDb(), ByteFromDb()) },
            { typeof(short), new CosmosDefaultValueConverter<short, short?>(ShortToDb(), ShortFromDb()) },
            { typeof(int), new CosmosDefaultValueConverter<int, int?>(IntToDb(), IntFromDb()) },
            { typeof(long), new CosmosDefaultValueConverter<long, long?>(LongToDb(), LongFromDb()) },
            { typeof(decimal), new CosmosDefaultValueConverter<decimal, decimal?>(DecimalToDb(), DecimalFromDb()) },
            { typeof(double), new CosmosDefaultValueConverter<double, double?>(DoubleToDb(), DoubleFromDb()) },
            { typeof(float), new CosmosDefaultValueConverter<float, float?>(FloatToDb(), FloatFromDb()) },
            { typeof(DateTime), new CosmosDefaultValueConverter<DateTime, DateTime?>(DateTimeToDb(), DateTimeFromDb()) },
            { typeof(DateTimeOffset), new CosmosDefaultValueConverter<DateTimeOffset, DateTimeOffset?>(DateTimeOffsetToDb(), DateTimeOffsetFromDb()) },
            { typeof(string), new CosmosDefaultValueConverter<string, string>(StringToDb(), StringFromDb()) },
            { typeof(Type), new TypeValueConverter() },
            {typeof(Object), new PolymorphicJsonValueConverter<object>() },
            {typeof(Object[]), new PolymorphicJsonValueConverter<object[]>() },
            {typeof(IEnumerable<Object[]>), new PolymorphicJsonValueConverter<object[]>() }

            // { typeof(SafeDictionary<string, object>), new CosmosDefaultValueConverter<SafeDictionary<string, object>, string>(SafeDictionaryToDb(), SafeDictionaryFromDb()) },
        };

        public static bool HasConverter(Type t)
        {
            return Converters?.ContainsKey(t) ?? false;
        }

        private static Expression<Func<bool?, bool>> BoolFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<bool, bool?>> BoolToDb() { return (i) => i; }

        private static Expression<Func<char?, char>> CharFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<char, char?>> CharToDb() { return (i) => i; }

        private static Expression<Func<byte?, byte>> ByteFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<byte, byte?>> ByteToDb() { return (i) => i; }

        private static Expression<Func<short?, short>> ShortFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<short, short?>> ShortToDb() { return (i) => i; }

        private static Expression<Func<int?, int>> IntFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<int, int?>> IntToDb() { return (i) => i; }

        private static Expression<Func<long?, long>> LongFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<long, long?>> LongToDb() { return (i) => i; }

        private static Expression<Func<decimal?, decimal>> DecimalFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<decimal, decimal?>> DecimalToDb() { return (i) => i; }

        private static Expression<Func<double?, double>> DoubleFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<double, double?>> DoubleToDb() { return (i) => i; }

        private static Expression<Func<float?, float>> FloatFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<float, float?>> FloatToDb() { return (i) => i; }

        private static Expression<Func<DateTime?, DateTime>> DateTimeFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<DateTime, DateTime?>> DateTimeToDb() { return (i) => i; }

        private static Expression<Func<DateTimeOffset?, DateTimeOffset>> DateTimeOffsetFromDb() { return (d) => d.HasValue ? d.Value : default; }

        private static Expression<Func<DateTimeOffset, DateTimeOffset?>> DateTimeOffsetToDb() { return (i) => i; }

        private static Expression<Func<string, string>> StringFromDb() { return (d) => d; }

        private static Expression<Func<string, string>> StringToDb() { return (i) => i; }

        //private static Expression<Func<string, SafeDictionary<string, object>>> SafeDictionaryFromDb()
        //{ return (d) => JsonConvert.DeserializeObject<SafeDictionary<string, object>>(d, new SafeObjectDictionaryJsonConverter()); }

        //private static Expression<Func<SafeDictionary<string, object>, string>> SafeDictionaryToDb()
        //{ return (i) => JsonConvert.SerializeObject(i, new SafeObjectDictionaryJsonConverter()); }
    }

    public class CosmosDefaultValueConverter<TModel, TProvider> : ValueConverter<TModel, TProvider>
    {


        public static CosmosDefaultValueConverter<long, long?> LongConverter = new CosmosDefaultValueConverter<long, long?>(
            (i) => i, (d) => d.HasValue ? d.Value : default);

        public CosmosDefaultValueConverter(
            Expression<Func<TModel, TProvider>> convertToProviderExpression,
            Expression<Func<TProvider, TModel>> convertFromProviderExpression,
            ConverterMappingHints mappingHints = null)
            : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
        {
        }
    }


}
