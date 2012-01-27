using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastic.Routing.RouteValues;

namespace Elastic.Routing
{
    /// <summary>
    /// A factory method container for route value providers.
    /// </summary>
    public static class RouteValue
    {
        /// <summary>
        /// Gets the value from the current request's route data.
        /// </summary>
        /// <returns>Returns the new instance of <see cref="OriginalRouteValueProvider"/> class.</returns>
        public static OriginalRouteValueProvider FromRequest()
        {
            return new OriginalRouteValueProvider();
        }

        /// <summary>
        /// Evaluates the value dynamically using the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns the new instance of <see cref="CallbackRouteValueProvider"/> class.</returns>
        public static CallbackRouteValueProvider Dynamic(Func<object> callback)
        {
            return new CallbackRouteValueProvider(callback);
        }

        /// <summary>
        /// Creates the route value projection which replaces all non-word characters in the outgoing route value by dashes.
        /// </summary>
        /// <param name="allowSlash">if set to <c>true</c> the '/' characters are not replaced with dashes.</param>
        /// <returns>Returns the new instance of <see cref="DashedRouteValueProjection"/> class.</returns>
        public static DashedRouteValueProjection DashedProjection(bool allowSlash)
        {
            return new DashedRouteValueProjection(allowSlash);
        }
    }
}
