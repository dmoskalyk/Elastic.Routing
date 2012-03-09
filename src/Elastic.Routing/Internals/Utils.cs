using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Globalization;

namespace Elastic.Routing.Internals
{
    /// <summary>
    /// A container class for internal utilities.
    /// </summary>
    public static class Utils
    {
        private static HashSet<UnicodeCategory> wordCharCategories = new HashSet<UnicodeCategory>()
        {
            UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter, 
            UnicodeCategory.OtherLetter, UnicodeCategory.ConnectorPunctuation,
            UnicodeCategory.ModifierLetter, UnicodeCategory.TitlecaseLetter, 
            UnicodeCategory.DecimalDigitNumber
        };

        /// <summary>
        /// Evaluates the default value for the current request.
        /// </summary>
        /// <param name="request">The request context.</param>
        /// <param name="defaults">The default route values.</param>
        /// <param name="key">The route value key.</param>
        /// <returns>Returns the default value or <c>null</c>.</returns>
        public static object EvaluateDefaultValue(RequestContext request, RouteValueDictionary defaults, string key)
        {
            var value = defaults[key];
            var valueProvider = value as IRouteValueProvider;
            if (valueProvider != null)
                value = valueProvider.GetValue(key, request);
            return value;
        }

        /// <summary>
        /// Replaces all non-word characters in the <paramref name="value"/> with dashes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="extraValidChars">The extra valid chars which are not replaced with dashes.</param>
        /// <returns>
        /// Returns the dashed value.
        /// </returns>
        public static string DashedValue(string value, HashSet<char> extraValidChars)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            var sb = new StringBuilder();
            bool isPrevDash = true;
            foreach (var ch in value)
            {
                if (wordCharCategories.Contains(Char.GetUnicodeCategory(ch)))
                {
                    sb.Append(ch);
                    isPrevDash = false;
                }
                else if (extraValidChars.Contains(ch))
                {
                    if (isPrevDash)
                        sb.Length--;
                    sb.Append(ch);
                    isPrevDash = false;
                }
                else if (!isPrevDash)
                {
                    sb.Append('-');
                    isPrevDash = true;
                }
            }
            if (sb.Length > 0 && isPrevDash)
                sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// Casts the specified route value dictionary in to the generic dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="dictionary">The route value dictionary.</param>
        /// <returns>Returns the strongly-typed dictionary.</returns>
        public static IDictionary<string, T> Cast<T>(this RouteValueDictionary dictionary)
        {
            if (dictionary == null)
                return null;

            var result = new Dictionary<string, T>();
            foreach (var entry in dictionary)
            {
                result.Add(entry.Key, (T)entry.Value);
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified dictionary has value.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified dictionary has value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValue(this RouteValueDictionary dictionary, string key, object value)
        {
            if (dictionary == null || value == null)
                return false;

            object v;
            if (!dictionary.TryGetValue(key, out v) || v == null)
                return false;

            return String.Equals(v.ToString(), value.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
