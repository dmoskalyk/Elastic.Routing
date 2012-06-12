using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// A literal path segment.
    /// </summary>
    public class LiteralPathSegment : PathSegment
    {
        /// <summary>
        /// Gets the text of the segment.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteralPathSegment"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public LiteralPathSegment(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the regex pattern for the current segment.
        /// </summary>
        /// <returns></returns>
        public override string GetRegexPattern()
        {
            return "(" + Regex.Escape(Text) + ")";
        }

        /// <summary>
        /// Extracts the route values of the current segment from the specified <paramref name="match"/>.
        /// If not overridden in a derived class, does not extract any values.
        /// </summary>
        /// <param name="match">The regex match to extract value from.</param>
        /// <param name="valueSetter">The route value setter used to extract value(s).</param>
        public override void ExtractRouteValues(Match match, Action<string, object> valueSetter)
        {
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
            return (SegmentValue)this.Text;
        }
    }
}
