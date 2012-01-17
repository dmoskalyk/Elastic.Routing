using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastic.Routing.RouteValueProviders;

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
        /// <returns>Returns the <see cref="OriginalRouteValueProvider"/>.</returns>
        public static OriginalRouteValueProvider FromRequest()
        {
            return new OriginalRouteValueProvider();
        }

        /// <summary>
        /// Evaluates the value dynamically using the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <returns>Returns the <see cref="CallbackRouteValueProvider"/>.</returns>
        public static CallbackRouteValueProvider Dynamic(Func<object> callback)
        {
            return new CallbackRouteValueProvider(callback);
        }
    }
}
