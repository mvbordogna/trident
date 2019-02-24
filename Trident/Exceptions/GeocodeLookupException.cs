using System;

namespace Trident.Exceptions
{
    /// <summary>
    /// Exception to be thrown when a geolocation service failes to resolve an address.
    /// </summary>
    public class GeocodeLookupException : Exception
    {
        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; }

        /// <summary>
        /// Gets the optional post code.
        /// </summary>
        /// <value>
        /// The post code.
        /// </value>
        public string PostCode { get; }

        /// <summary>
        /// Gets the optional country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; }

        /// <summary>
        /// Gets the optional radius meters.
        /// </summary>
        /// <value>
        /// The radius meters.
        /// </value>
        public double? RadiusMeters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeLookupException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="address">The address.</param>
        /// <param name="radiusMeters">The radius meters.</param>
        public GeocodeLookupException(string message, string address, double? radiusMeters) : base(message)
        {
            Address = address;
            RadiusMeters = radiusMeters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeocodeLookupException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="address">The address.</param>
        /// <param name="postCode">The post code.</param>
        /// <param name="countryCode">The country code.</param>
        public GeocodeLookupException(string message, string address, string postCode, string countryCode) : base(message)
        {
            Address = address;
            PostCode = postCode;
            CountryCode = countryCode;
        }
    }
}