using Azure.Core.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Text.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Trident.Extensions;

namespace Trident.Azure
{
    public class JsonNetObjectSerializer : JsonObjectSerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }            
        };

        public JsonNetObjectSerializer() : base()
        {
            settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
        }

        public JsonNetObjectSerializer(JsonSerializerOptions options) : base(options)
        {

        }

        public override object Deserialize(Stream stream, Type returnType, CancellationToken cancellationToken)
        {
            var str = ReadStream(stream);
            if (returnType.IsPrimitive())
            {
               return str.ConvertTo(returnType);
            }
            else
            {
                return JsonConvert.DeserializeObject(str, returnType, settings);
            }
        }

        public override async ValueTask<object> DeserializeAsync(Stream stream, Type returnType, CancellationToken cancellationToken)
        {
            return JsonConvert.DeserializeObject(await ReadStreamAsync(stream), returnType, settings);
        }

        public override BinaryData Serialize(object value, Type inputType = null, CancellationToken cancellationToken = default)
        {
            return new BinaryData(JsonConvert.SerializeObject(value, settings));
        }


        public override void Serialize(Stream stream, object value, Type inputType, CancellationToken cancellationToken)
        {
            WriteStream(stream, JsonConvert.SerializeObject(value, settings));
        }

        public override ValueTask<BinaryData> SerializeAsync(object value, Type inputType = null, CancellationToken cancellationToken = default)
        {
            return ValueTask.FromResult(new BinaryData(JsonConvert.SerializeObject(value, settings)));
        }

        public override async ValueTask SerializeAsync(Stream stream, object value, Type inputType, CancellationToken cancellationToken)
        {
            await WriteStreamAsync(stream, JsonConvert.SerializeObject(value, settings));
        }

        private async Task WriteStreamAsync(Stream stream, string value)
        {
            var sw = new StreamWriter(stream);
            await sw.WriteAsync(value);
            await sw.FlushAsync();
        }

        private void WriteStream(Stream stream, string value)
        {
            var sw = new StreamWriter(stream);
            sw.Write(value);
            sw.Flush();
        }

        private async Task<string> ReadStreamAsync(Stream stream)
        {
            var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync();
        }

        private string ReadStream(Stream stream)
        {
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

    }

}
