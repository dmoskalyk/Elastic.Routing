using System.Text.RegularExpressions;
using Elastic.Routing.Parsing;

namespace Elastic.Routing
{
    internal class ParsedRoutePattern
    {
        public readonly Regex UrlMatch;
        public readonly FullPathSegment FullPathSegment;

        public ParsedRoutePattern(Regex urlMatch, FullPathSegment fullPathSegment)
        {
            UrlMatch = urlMatch;
            FullPathSegment = fullPathSegment;
        }
    }
}
