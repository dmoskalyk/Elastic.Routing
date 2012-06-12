using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// Represents the entire URL path.
    /// </summary>
    public class FullPathSegment : PathSegment
    {
        /// <summary>
        /// Gets the hash set of all parameters in the path.
        /// </summary>
        public HashSet<string> Parameters { get; private set; }

        /// <summary>
        /// Gets the list of the required parameters.
        /// </summary>
        public IList<ParameterPathSegment> RequiredParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullPathSegment"/> class.
        /// </summary>
        /// <param name="segments">The child segments.</param>
        /// <param name="parameters">The hash set of all parameters in the path.</param>
        public FullPathSegment(IEnumerable<PathSegment> segments, HashSet<string> parameters)
        {
            this.Parameters = parameters;
            this.Segments.AddRange(segments);
            this.RequiredParameters = this.Segments.OfType<ParameterPathSegment>().ToArray();
        }

        /// <summary>
        /// Gets the regex pattern for the current segment.
        /// </summary>
        /// <returns></returns>
        public override string GetRegexPattern()
        {
            var pattern = String.Concat(Segments.Select(s => s.GetRegexPattern()));
            return '^' + pattern + '$';
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
        /// If any of the child parts is <c>null</c>, the result is also <c>null</c>.
        /// </summary>
        /// <param name="valueGetter">The route value getter delegate.</param>
        /// <returns>
        /// Returns the corresponding part of the URL or <c>null</c> when the value is missing.
        /// </returns>
        public override SegmentValue GetUrlPart(Func<string, SegmentValue> valueGetter)
        {
            var parts = Segments.Select(s => s.GetUrlPart(valueGetter)).ToList();
            if (parts.Any(p => p == null))
                return null;
            return (SegmentValue)string.Concat(parts.Select(s => s.ToString()));
        }
    }
}
