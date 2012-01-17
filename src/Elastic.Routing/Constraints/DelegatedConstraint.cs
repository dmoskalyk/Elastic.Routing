using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace Elastic.Routing.Constraints
{
    /// <summary>
    /// A constraint which delegates the decision making to the specified predicate.
    /// </summary>
    public sealed class DelegatedConstraint : IRouteConstraint
    {
        Predicate<string> predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatedConstraint"/> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaludate the constraint.</param>
        public DelegatedConstraint(Predicate<string> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            this.predicate = predicate;
        }

        /// <summary>
        /// Determines whether the URL parameter contains a valid value for this constraint.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The object that this constraint belongs to.</param>
        /// <param name="parameterName">The name of the parameter that is being checked.</param>
        /// <param name="values">An object that contains the parameters for the URL.</param>
        /// <param name="routeDirection">An object that indicates whether the constraint check is being performed when an incoming request is being handled or when a URL is being generated.</param>
        /// <returns>
        /// true if the URL parameter contains a valid value; otherwise, false.
        /// </returns>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var value = (string)values[parameterName];
            return predicate(value);
        }
    }
}
