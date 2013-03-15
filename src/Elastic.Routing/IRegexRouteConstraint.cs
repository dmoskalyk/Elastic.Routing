using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing
{
    /// <summary>
    /// An interface representing a route constraint which can also provide a regular expression pattern in addition to custom constrain logic.
    /// </summary>
    public interface IRegexRouteConstraint : IRouteConstraint
    {
        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        /// <value>
        /// The regular expression pattern string.
        /// </value>
        string Pattern { get; }
    }
}
