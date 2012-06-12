using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Routing;
using System.Collections.ObjectModel;
using System.Web;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// The base class for different types of the template path segments.
    /// </summary>
    public abstract class PathSegment
    {
        /// <summary>
        /// Gets the child segments.
        /// </summary>
        public PathSegmentCollection Segments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathSegment"/> class.
        /// </summary>
        public PathSegment()
        {
            this.Segments = new PathSegmentCollection();
        }

        /// <summary>
        /// Gets the regex pattern for the current segment.
        /// </summary>
        /// <returns></returns>
        public abstract string GetRegexPattern();

        /// <summary>
        /// Extracts the route values of the current segment from the specified <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The regex match to extract value from.</param>
        /// <param name="valueSetter">The route value setter used to extract value(s).</param>
        public abstract void ExtractRouteValues(Match match, Action<string, object> valueSetter);

        /// <summary>
        /// Gets the URL part for this segment. Used in URLs construction.
        /// </summary>
        /// <param name="valueGetter">The route value getter delegate.</param>
        /// <returns>Returns the corresponding part of the URL or <c>null</c> when the value is missing.</returns>
        public abstract SegmentValue GetUrlPart(Func<string, SegmentValue> valueGetter);
    }

    /// <summary>
    /// The collection of path segments.
    /// </summary>
    public class PathSegmentCollection : Collection<PathSegment>
    {
        /// <summary>
        /// Adds the range of items to the collection.
        /// </summary>
        /// <param name="collection">The collection of items to add.</param>
        public void AddRange(IEnumerable<PathSegment> collection)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }
    }
}
