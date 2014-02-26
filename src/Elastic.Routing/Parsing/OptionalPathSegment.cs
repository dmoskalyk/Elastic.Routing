using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// Represents the optinal part of the path.
    /// </summary>
    public class OptionalPathSegment : PathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalPathSegment"/> class.
        /// </summary>
        public OptionalPathSegment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalPathSegment"/> class.
        /// </summary>
        /// <param name="segments">The child segments.</param>
        public OptionalPathSegment(IEnumerable<PathSegment> segments)
        {
            this.Segments.AddRange(segments);
        }

        /// <summary>
        /// Gets the regex pattern for the current segment.
        /// </summary>
        /// <returns></returns>
        public override string GetRegexPattern()
        {
            var pattern = String.Concat(Segments.Select(s => s.GetRegexPattern()));
            if (String.IsNullOrEmpty(pattern))
                return pattern;
            if (Segments.Count > 1)
                pattern = '(' + pattern + ')';
            return pattern + "?";
        }

        /// <summary>
        /// Extracts the route values of the current segment from the specified <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The regex match to extract value from.</param>
        /// <param name="valueSetter">The route value setter used to extract value(s).</param>
        public override void ExtractRouteValues(Match match, Action<string, object> valueSetter)
        {
            foreach (var segment in Segments)
            {
                segment.ExtractRouteValues(match, valueSetter);
            }
        }

        /// <summary>
        /// Gets the URL part for this segment. Used in URLs construction.
        /// If any of the child parts is <c>null</c>, returns an empty string.
        /// </summary>
        /// <param name="valueGetter">The route value getter delegate.</param>
        /// <returns>
        /// Returns the corresponding part of the URL or <c>null</c> when the value is missing.
        /// </returns>
        public override SegmentValue GetUrlPart(Func<string, SegmentValue> valueGetter)
        {
            var parts = Segments.Select(s => s.GetUrlPart(valueGetter)).ToList();
            if (parts.Any(p => p == null) || parts.Count == 0)
                return SegmentValue.Empty;

            if (ShouldBeEmpty(parts))
                return SegmentValue.Empty;
            var result = string.Concat(parts.Select(s => s.ToString()));
            return SegmentValue.Create(result);
        }

        private bool ShouldBeEmpty(IList<SegmentValue> parts)
        {
            var zip = Segments.Zip(parts, (s, p) => new { Segment = s, Value = p }).ToList();
            bool hasParameters = false;
            foreach (var item in zip)
            {
                if (item.Segment is ParameterPathSegment)
                {
                    hasParameters = true;
                    if (!item.Value.IsDefault)
                        return false;
                }
            }
            return hasParameters;
        }
    }
}
