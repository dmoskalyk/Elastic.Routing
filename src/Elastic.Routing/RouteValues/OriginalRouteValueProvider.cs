using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing.RouteValues
{
    /// <summary>
    /// The route value provider which gets the value from the current request's route data.
    /// </summary>
    public sealed class OriginalRouteValueProvider : IRouteValueProvider
    {
        /// <summary>
        /// Gets the value from the specified request's route data.
        /// </summary>
        /// <param name="key">The value key.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// Returns the value for the specified key.
        /// </returns>
        public object GetValue(string key, RequestContext request)
        {
            return request.RouteData.Values[key];
        }
    }
}
