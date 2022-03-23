using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Trident.Extensions
{
    /// <summary>
    /// Class EnumExtensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the display value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayValue(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
            typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        /// <summary>
        /// Gets the description attribute of the enumeration value.
        /// </summary>
        /// <param name="e">The specified enumeration value.</param>
        /// <returns>The description attribute of the enumeration value.</returns>
        public static string GetDescription(this Enum e)
        {
            return EnumExtensions.GetEnumDescription<Enum>(e);
        }

        /// <summary>
        /// Gets the description attribute of the enumeration type.
        /// </summary>
        /// <typeparam name="TEnum">The type of enumeration.</typeparam>
        /// <param name="value">The specified enumeration value.</param>
        /// <returns>he description attribute of the enumeration value.</returns>
        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            if (value == null)
                return "";

            var enunStrValue = value?.ToString() ?? "";
            var enumField = value.GetType().GetField(enunStrValue);
            DescriptionAttribute attr = enumField?
                .GetCustomAttribute<DescriptionAttribute>(false);

            return attr?.Description ?? enunStrValue.ToSentenceCase(true);
        }

        /// <summary>
        /// Retrieves an array of description attributes for the specified enumeration type.
        /// </summary>
        /// <param name="enumType">The specified enumeration type.</param>
        /// <returns>An array of description attributes for the specified enumeration type.</returns>
        public static string[] GetDescriptions(Type enumType)
        {
            Array values = Enum.GetValues(enumType);
            string[] strArray = new string[values.Length];
            for (int index = 0; index < values.Length; ++index)
                strArray[index] = EnumExtensions.GetEnumDescription<object>(values.GetValue(index));
            return strArray;
        }

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>IEnumerable&lt;Enum&gt;.</returns>
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

    }
}
