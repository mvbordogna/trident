using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace Trident.Data.RestSharp
{
    /// <summary>
    /// Class NewtonsoftJsonSerializer.
    /// Implements the <see cref="RestSharp.Serializers.ISerializer" />
    /// Implements the <see cref="RestSharp.Deserializers.IDeserializer" />
    /// </summary>
    /// <seealso cref="RestSharp.Serializers.ISerializer" />
    /// <seealso cref="RestSharp.Deserializers.IDeserializer" />
    /// <summary>
    /// Class NewtonsoftJsonSerializer.
    /// Implements the <see cref="RestSharp.Serializers.ISerializer" />
    /// Implements the <see cref="RestSharp.Deserializers.IDeserializer" />
    /// </summary>
    /// <seealso cref="RestSharp.Serializers.ISerializer" />
    /// <seealso cref="RestSharp.Deserializers.IDeserializer" />
    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        /// <summary>
        /// The default
        /// </summary>
        /// <summary>
        /// The default
        /// </summary>
        public static readonly NewtonsoftJsonSerializer Default = new NewtonsoftJsonSerializer();

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public JsonSerializerSettings Settings { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer" /> class.
        /// </summary>
        public NewtonsoftJsonSerializer()
        {
            Settings = new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        /// <summary>
        /// Deserializes the specified response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns>T.</returns>
        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content, Settings);
        }

        /// <summary>
        /// Gets or sets the root element.
        /// </summary>
        /// <value>The root element.</value>
        public string RootElement { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>The date format.</value>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; set; }
    }
}