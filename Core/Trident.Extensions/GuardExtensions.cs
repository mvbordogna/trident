using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trident.Extensions
{
    /// <summary>
    /// Class GuardExtensions provides helper extension methods for general pointer and value type states
    /// where if expectation is not met a argument exceptions is thrown.
    /// </summary>
    public static class GuardExtensions
    {
        /// <summary>
        /// Guards the is not null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardIsNotNull(this object parameter, string parameterName, string message=null)
        {
            if (null == parameter)
            {
                throw new ArgumentNullException(parameterName, message ?? string.Empty);
            }
        }


        /// <summary>
        /// Guards the is not null or empty.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsNotNullOrEmpty(this string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be null or empty.", parameterName), parameterName);
            }
        }

        /// <summary>
        /// Throws argument exception if target is null or whitespace.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsNotNullOrWhitespace(this string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} cannot be null or empty.", parameterName), parameterName);
            }
        }

        /// <summary>
        /// Guards the is null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException">Argument must be null.</exception>
        public static void GuardIsNull(this object parameter, string parameterName)
        {
            if (null != parameter)
            {
                throw new ArgumentException("Argument must be null.", parameterName);
            }
        }


        /// <summary>
        /// Guards the are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardAreEqual<T>(this T expectedValue, T value, string parameterName) where T : IComparable
        {
            if (0 != expectedValue.CompareTo(value))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} does not equal the expected value of {1}.", value, expectedValue), parameterName);
            }
        }

        /// <summary>
        /// Guards the type of the are same.
        /// </summary>
        /// <param name="expectedValue">The expected value.</param>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardAreSameType(this Type expectedValue, Type value, string parameterName)
        {
            if (value == null || expectedValue != value)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} does not equal the expected value of {1}.", value, expectedValue), parameterName);
            }
        }

        /// <summary>
        /// Guards the is not empty unique identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsNotEmptyGuid(this Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is an empty Guid.", parameterName));
            }
        }

        /// <summary>
        /// Guards the is null or empty unique identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsNullOrEmptyGuid(this Guid? value, string parameterName)
        {
            if (value != null && value.GetValueOrDefault() != Guid.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a null or empty Guid.", parameterName));
            }
        }

        /// <summary>
        /// Guards the is not null or empty unique identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsNotNullOrEmptyGuid(this Guid? value, string parameterName)
        {
            if (value == null || value.GetValueOrDefault() == Guid.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not a null or empty Guid.", parameterName));
            }
        }


        /// <summary>
        /// Guards the is empty unique identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        public static void GuardIsEmptyGuid(this Guid value, string parameterName)
        {
            if (value != Guid.Empty)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} is not an empty Guid.", parameterName));
            }
        }


        /// <summary>
        /// Guards the is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardIsNullOrDefaultValue<T>(this T? parameter, string parameterName) where T : struct
        {
            if (null == parameter || parameter.Value.Equals(default(T)))
            {
                return;
            }
            throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Guards it is not zero or less.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardIsGreaterThanZero(this int parameter, string parameterName)
        {
            if (parameter <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} must be greater than zero.", parameterName));
            }
        }

        /// <summary>
        /// Guards it is not zero or less.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardIsGreaterThanZero(this long parameter, string parameterName)
        {
            if (parameter <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} must be greater than zero.", parameterName));
            }
        }

        /// <summary>
        /// Guards it is not zero or less.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardIsZero(this long parameter, string parameterName)
        {
            if (parameter != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} must be zero.", parameterName));
            }
        }

        /// <summary>
        /// Guards that the emum's value is on the Undefined value of the enum.
        /// Your enum must have a member of Undefined = 0 to use this method.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardNotUndefined(this Enum parameter, string parameterName)
        {
            if (parameter.ToString() == "Undefined")
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void GuardHasNoNulls<T>(this IList<T> value, string parameterName)
           where T : class
        {
            value.GuardIsNotNull(parameterName);

            var i = 0;

            foreach(var v in value)
            {
                v.GuardIsNotNull(parameterName, $"Contains a null index at {i}");
                    i++;
            }
        }
    }
}
