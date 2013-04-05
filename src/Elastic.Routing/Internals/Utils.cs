using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Globalization;
using System.Text.RegularExpressions;

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
        /// <param name="maxLength">The maximum length of the result.</param>
        /// <returns>
        /// Returns the dashed value.
        /// </returns>
        public static string DashedValue(string value, HashSet<char> extraValidChars, int maxLength = 0)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            var sb = new StringBuilder();
            bool isPrevSep = true;
            foreach (var ch in value)
            {
                if (wordCharCategories.Contains(Char.GetUnicodeCategory(ch)))
                {
                    sb.Append(ch);
                    isPrevSep = false;
                }
                else if (extraValidChars.Contains(ch))
                {
                    bool isPrevDash = isPrevSep && sb[sb.Length - 1] == '-';
                    if (isPrevDash)
                        sb.Length--;
                    if (!isPrevSep || isPrevDash)
                        sb.Append(ch);
                    isPrevSep = true;
                }
                else if (!isPrevSep)
                {
                    sb.Append('-');
                    isPrevSep = true;
                }
            }
            if (sb.Length > 0 && isPrevSep)
                sb.Length--;

            var s = sb.ToString();
            if (maxLength > 0 && sb.Length > maxLength)
            {
                s = s.Substring(0, maxLength);
                if (sb[maxLength] != '-')
                {
                    var prevDashIndex = s.LastIndexOf('-');
                    if (prevDashIndex > 0)
                        s = s.Remove(prevDashIndex);
                }
            }
            return s.Trim('_');
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

        /// <summary>
        /// Copies the entries from the source dictionary to the target.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="target">The target.</param>
        public static void CopyTo<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> target)
        {
            if (source != null)
            {
                foreach (var entry in source)
                {
                    target[entry.Key] = entry.Value;
                }
            }
        }

        /// <summary>
        /// Converts the object to the route values dictionary.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>Returns the route values dictionary or <c>null</c> if the <paramref name="data"/> is <c>null</c>.</returns>
        public static RouteValueDictionary ToRouteValues(this object data)
        {
            if (data == null)
                return null;
            else if (data is RouteValueDictionary)
                return (RouteValueDictionary)data;
            else if (data is IDictionary<string, object>)
                return new RouteValueDictionary((IDictionary<string, object>)data);
            else
                return new RouteValueDictionary(data);
        }

        /// <summary>
        /// Encodes the route value to be used in the URL.
        /// Replicates the behavior of the standard <see cref="System.Web.Routing.Route"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns the URL-encoded value.</returns>
        public static string UrlEncode(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(Uri.EscapeUriString(value), "([#;?:@&=+$,])",
                m => "%" + Convert.ToUInt16(m.Value[0]).ToString("x2", CultureInfo.InvariantCulture));
        }
    }
}
