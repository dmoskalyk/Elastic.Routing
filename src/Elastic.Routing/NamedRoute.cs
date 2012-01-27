using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Collections.ObjectModel;

namespace Elastic.Routing
{
    /// <summary>
    /// A class incapsulating the route with its name.
    /// </summary>
    /// <typeparam name="TRoute">The type of the route.</typeparam>
    public class NamedRoute<TRoute>
        where TRoute : RouteBase
    {
        /// <summary>
        /// Gets or sets the route name.
        /// </summary>
        /// <value>
        /// The route name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        /// <value>
        /// The route.
        /// </value>
        public TRoute Route { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedRoute&lt;TRoute&gt;"/> class.
        /// </summary>
        public NamedRoute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedRoute&lt;TRoute&gt;"/> class.
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="route">The route.</param>
        public NamedRoute(string routeName, TRoute route)
        {
            this.Name = routeName;
            this.Route = route;
        }
    }

    /// <summary>
    /// The collection of named routes.
    /// </summary>
    /// <typeparam name="TRoute">The type of the route.</typeparam>
    public class NamedRouteCollection<TRoute> : Collection<NamedRoute<TRoute>>
        where TRoute : RouteBase
    {
        /// <summary>
        /// Adds the specified route name.
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="route">The route.</param>
        public void Add(string routeName, TRoute route)
        {
            this.Add(new NamedRoute<TRoute>(routeName, route));
        }

        /// <summary>
        /// Finds the route its name.
        /// </summary>
        /// <param name="routeName">Name of the route.</param>
        /// <returns>Returns the found route or <c>null</c>.</returns>
        public TRoute Find(string routeName)
        {
            return this.Where(nr => nr.Name == routeName).Select(i => i.Route).FirstOrDefault();
        }
    }
}
