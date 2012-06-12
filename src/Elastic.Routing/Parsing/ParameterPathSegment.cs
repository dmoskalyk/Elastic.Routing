using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// Represents the path parameter.
    /// </summary>
    public class ParameterPathSegment : PathSegment
    {
        private string pattern;

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterPathSegment"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="customPattern">The custom regex pattern.</param>
        public ParameterPathSegment(string name, string customPattern = null)
        {
            this.Name = name;
            this.pattern = customPattern ?? GetDefaultRegexPattern();
        }

        /// <summary>
        /// Gets the regex pattern for the current segment.
        /// </summary>
        /// <returns></returns>
        public sealed override string GetRegexPattern()
        {
            return @"(?<" + Name + @">" + pattern + ")";
        }

        /// <summary>
        /// Extracts the route values of the current segment from the specified <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The regex match to extract value from.</param>
        /// <param name="valueSetter">The route value setter used to extract value(s).</param>
        public override void ExtractRouteValues(Match match, Action<string, object> valueSetter)
        {
            var group = match.Groups[Name];
            valueSetter(Name, group.Success ? group.Value : null);
        }

        /// <summary>
        /// Gets the default regex pattern if the custom one has not been passed.
        /// </summary>
        /// <returns>Returns the pattern which matches 1 or more characters except '/'.</returns>
        protected virtual string GetDefaultRegexPattern()
        {
            return "[^/]+";
        }

        /// <summary>
        /// Gets the URL part for this segment. Used in URLs construction.
        /// </summary>
        /// <param name="valueGetter">The route value getter delegate.</param>
        /// <returns>
        /// Returns the corresponding part of the URL or <c>null</c> when the value is missing.
        /// </returns>
        public override SegmentValue GetUrlPart(Func<string, SegmentValue> valueGetter)
        {
            var value = valueGetter(Name);
            value = SegmentValue.Create(HttpUtility.UrlEncode((string)value), value != null && value.IsDefault);
            return MatchesPattern((string)value) ? value : null;
        }

        /// <summary>
        /// Matches the specified value against the current pattern.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value indicating whether the the match is successful.</returns>
        protected virtual bool MatchesPattern(string value)
        {
            return (value != null && Regex.IsMatch(value, "^" + pattern + "$"));
        }
    }
}
