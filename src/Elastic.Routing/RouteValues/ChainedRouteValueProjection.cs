using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastic.Routing.RouteValues
{
    /// <summary>
    /// A route value projection which encapsulates the ordered list of inner projections.
    /// </summary>
    public class ChainedRouteValueProjection : IRouteValueProjection
    {
        IList<IRouteValueProjection> projections;

        /// <summary>
        /// Gets the inner projections.
        /// </summary>
        public IList<IRouteValueProjection> Projections
        {
            get { return projections; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainedRouteValueProjection"/> class.
        /// For incoming route values the projections are evaluated in the direct order, for outgoing - in the reverse order.
        /// </summary>
        /// <param name="projections">The projections.</param>
        public ChainedRouteValueProjection(IEnumerable<IRouteValueProjection> projections)
        {
            this.projections = projections.ToList().AsReadOnly();
        }

        /// <summary>
        /// Transforms the incoming route value using the inner projections in the direct order.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Incoming(string key, System.Web.Routing.RouteValueDictionary values)
        {
            foreach (var projection in projections)
            {
                projection.Incoming(key, values);
            }
        }

        /// <summary>
        /// Transforms the outgoing route value using the inner projections in the reverse order.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Outgoing(string key, System.Web.Routing.RouteValueDictionary values)
        {
            foreach (var projection in projections.Reverse())
            {
                projection.Outgoing(key, values);
            }
        }
    }
}
