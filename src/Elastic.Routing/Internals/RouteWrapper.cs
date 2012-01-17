using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing.Internals
{
    /// <summary>
    /// A wrapper class for the <see cref="ElasticRoute"/> to be passed into the custom <see cref="IRouteConstraint"/> objects.
    /// </summary>
    public sealed class RouteWrapper : Route
    {
        /// <summary>
        /// Gets the original route.
        /// </summary>
        public ElasticRoute OriginalRoute { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteWrapper"/> class.
        /// </summary>
        /// <param name="route">The original route.</param>
        public RouteWrapper(ElasticRoute route)
            : base(null, route.RouteHandler)
        {
            this.OriginalRoute = route;
        }
    }
}
