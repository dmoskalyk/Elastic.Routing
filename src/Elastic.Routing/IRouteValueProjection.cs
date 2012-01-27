using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing
{
    /// <summary>
    /// An interface for the projections which transforms the route values when parsing an incoming or generating am outgoing URL.
    /// </summary>
    public interface IRouteValueProjection
    {
        /// <summary>
        /// Transforms the incoming route value.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        void Incoming(string key, RouteValueDictionary values);

        /// <summary>
        /// Transforms the outgoing route value.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        void Outgoing(string key, RouteValueDictionary values);
    }
}
