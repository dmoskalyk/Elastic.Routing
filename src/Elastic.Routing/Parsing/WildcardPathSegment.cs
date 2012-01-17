using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastic.Routing.Parsing
{
    /// <summary>
    /// Represents the parameter path segment which can match a URL part also with '/' character.
    /// </summary>
    public class WildcardPathSegment : ParameterPathSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WildcardPathSegment"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="customPattern">The custom regex pattern.</param>
        public WildcardPathSegment(string name, string customPattern = null)
            : base(name, customPattern)
        {
        }

        /// <summary>
        /// Gets the default regex pattern if the custom one has not been passed.
        /// </summary>
        /// <returns>
        /// Returns the pattern which matches 1 or more characters.
        /// </returns>
        protected override string GetDefaultRegexPattern()
        {
            return ".+";
        }
    }
}
