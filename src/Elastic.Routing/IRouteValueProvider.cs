using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing
{
    /// <summary>
    /// An provider interface for retrieving the route value from the custom source.
    /// </summary>
    public interface IRouteValueProvider
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The value key.</param>
        /// <param name="request">The request.</param>
        /// <returns>Returns the value for the specified key.</returns>
        object GetValue(string key, RequestContext request);
    }
}
