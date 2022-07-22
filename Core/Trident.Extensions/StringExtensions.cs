using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace Trident.Extensions
{
    /// <summary>
    /// Extension methods for string.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Adds space before upper case character in the specified string.
        /// </summary>
        /// <param name="s">The specified string to process.</param>
        /// <returns>A string value with space before upper case character.</returns>
        public static string SplitByUppercase(this string s)
        {
            MatchCollection matchCollection = Regex.Matches(s, "(\\p{Nd}+)|(\\P{Lu}+)|(\\p{Lu}+\\p{Ll}*)");
            string str = "";
            foreach (Match match in matchCollection)
                str = str + match.ToString() + " ";
            return str.TrimEnd(' ');
        }

        public static Dictionary<string, string> ToDictionary(this string target, char pairDelimiter, char keyValueDelimiter, bool lowerCaseKeys = false)
        {
            var pairs = target.Split(pairDelimiter)
                .Select(x =>
                {
                    var pair = x.Split(new char[] { keyValueDelimiter }, 2);
                    var key = pair.First()?.Trim();

                    if (lowerCaseKeys) key = key.ToLower();

                    return new KeyValuePair<string, string>(key, pair.Last()?.Trim());
                }).ToDictionary(x => x.Key, x => x.Value);

            return new Dictionary<string, string>(pairs, StringComparer.InvariantCultureIgnoreCase);
        }


        /// <summary>
        /// Formats the specified string to sentence case.
        /// </summary>
        /// <param name="s">The specified string to format.</param>
        /// <param name="splitByUppercase">True to add space before upper case character, otherwise not.</param>
        /// <returns>A string value formatted to sentence case.</returns>
        public static string ToSentenceCase(this string s, bool splitByUppercase = false)
        {
            string str = splitByUppercase ? s.SplitByUppercase() : s;
            if (str.Length == 0)
                return str;
            return str.Substring(0, 1) + str.Substring(1).ToLower();
        }

        /// <summary>
        /// Converts to securestring.
        /// </summary>
        /// <param name="stringToSecure">Converts to secure.</param>
        /// <returns>SecureString.</returns>
        public static SecureString ToSecureString(this string stringToSecure)
        {
            SecureString sec_pass = new SecureString();
            Array.ForEach(stringToSecure.ToArray(), sec_pass.AppendChar);
            sec_pass.MakeReadOnly();
            return sec_pass;
        }


        /// <summary>
        /// Converts to unsecurestring.
        /// </summary>
        /// <param name="securePassword">The secure password.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">securePassword</exception>
        public static string ToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string FromBase64(this string encoded)
        {
            byte[] bytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string ToBase64(this string unencoded)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(unencoded);
            return Convert.ToBase64String(bytes);
        }

    }
}

