using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Trident.Contracts.Api.Validation
{
    /// <summary>
    /// ErrorCodes
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ErrorCodes
    {
        /// <summary>
        /// Error code used for unit testing.
        /// </summary>
        [Description("General Exception Error Code.")]
        Exception,

        /// <summary>
        /// Error code used for unit testing.
        /// </summary>
        [Description("Test Code Message-needed for unit tests.")]
        TestCode,

        /// <summary>
        /// Error code used for Required Field.
        /// </summary>
        [Description("Required Field."), JsonConverter(typeof(StringEnumConverter))]
        E10,

        /// <summary>
        /// Error code for invalid character length
        /// </summary>
        [Description("Invalid character length."), JsonConverter(typeof(StringEnumConverter))]
        E15,

        /// <summary>
        /// Error code used for String and Special Characters Only.
        /// </summary>
        [Description("String and Special Characters Only."), JsonConverter(typeof(StringEnumConverter))]
        E20,

        /// <summary>
        /// Error code used for Alphanumeric Characters Only.
        /// </summary>
        [Description("Alphanumeric Characters Only."), JsonConverter(typeof(StringEnumConverter))]
        E25,

        /// <summary>
        /// Error code used for Alpha Characters Only.
        /// </summary>
        [Description("Alpha Characters Only."), JsonConverter(typeof(StringEnumConverter))]
        E26,

        /// <summary>
        /// Error code used for Invalid Email Format.
        /// </summary>
        [Description("Invalid Email Format."), JsonConverter(typeof(StringEnumConverter))]
        E30,

        /// <summary>
        /// Error code used for Phone Format.
        /// </summary>
        [Description("Invalid Phone Format."), JsonConverter(typeof(StringEnumConverter))]
        E40,

        /// <summary>
        /// Error code used for Invalid Format.
        /// </summary>
        [Description("Invalid Format."), JsonConverter(typeof(StringEnumConverter))]
        E50,

        /// <summary>
        /// Error code used for exact match.
        /// </summary>
        [Description("Exact match found."), JsonConverter(typeof(StringEnumConverter))]
        E60,
        /// <summary>
        /// Error code used for duplicate email.
        /// </summary>
        [Description("Duplicate email found."), JsonConverter(typeof(StringEnumConverter))]
        E70,
        /// <summary>
        /// Error code used for duplicate phone number.
        /// </summary>
        [Description("Duplicate phone number found."), JsonConverter(typeof(StringEnumConverter))]
        E80,
        /// <summary>
        /// Error code used for invalid address.
        /// </summary>
        [Description("Invalid address."), JsonConverter(typeof(StringEnumConverter))]
        E90,
    }
}

