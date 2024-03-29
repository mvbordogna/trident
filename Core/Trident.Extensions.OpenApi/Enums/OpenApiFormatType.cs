using Trident.Extensions.OpenApi.Attributes;

namespace Trident.Extensions.OpenApi.Enums
{
    /// <summary>
    /// This specifies the Open API format.
    /// </summary>
    public enum OpenApiFormatType
    {
        /// <summary>
        /// Identifies the JSON format.
        /// </summary>
        [Display("json")]
        Json = 0,

        /// <summary>
        /// Identifies the YAML format.
        /// </summary>
        [Display("yaml")]
        Yaml = 1
    }
}
