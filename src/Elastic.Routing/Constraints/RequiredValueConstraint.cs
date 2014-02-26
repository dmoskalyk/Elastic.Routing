using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace Elastic.Routing.Constraints
{
    /// <summary>
    /// A constraint which requires a specific value to be present during the URL construction.
    /// </summary>
    public sealed class RequiredValueConstraint : IRouteConstraint
    {
        /// <summary>
        /// Gets the expected value.
        /// </summary>
        /// <value>
        /// The expected value.
        /// </value>
        public string ExpectedValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValueConstraint"/> class.
        /// </summary>
        /// <param name="value">The expected value of the parameter.</param>
        public RequiredValueConstraint(string value = null)
        {
            this.ExpectedValue = value;
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
            if (routeDirection == RouteDirection.UrlGeneration)
            {
                var value = values[parameterName];
                return value != null && (ExpectedValue == null || string.Equals(ExpectedValue, (string)value, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return false;
            }
        }
    }
}
