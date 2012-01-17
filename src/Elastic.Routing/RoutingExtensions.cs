using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing
{
    /// <summary>
    /// A container for route registration extension methods.
    /// </summary>
    public static class RoutingExtensions
    {
        /// <summary>
        /// Adds a new <see cref="ElasticRoute"/> to the collection.
        /// </summary>
        /// <param name="routes">The routes collection.</param>
        /// <param name="url">The URL pattern.</param>
        /// <param name="routeHandler">The route handler.</param>
        /// <param name="constraints">The route values constraints.</param>
        /// <param name="incomingDefaults">The incoming default route values.</param>
        /// <param name="outgoingDefaults">The outgoing default route values.</param>
        /// <param name="routeName">Name of the route to be registered as.</param>
        public static void Map(this RouteCollection routes, string url, IRouteHandler routeHandler, object constraints = null,
            object incomingDefaults = null, object outgoingDefaults = null, string routeName = null)
        {
            var route = new ElasticRoute(url,
                routeHandler: routeHandler,
                constraints: constraints != null ? new RouteValueDictionary(constraints) : null,
                incomingDefaults: incomingDefaults != null ? new RouteValueDictionary(incomingDefaults) : null,
                outgoingDefaults: outgoingDefaults != null ? new RouteValueDictionary(outgoingDefaults) : null);
            if (routeName != null)
                routes.Add(routeName, route);
            else
                routes.Add(route);
        }
    }
}
