using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing.RouteValueProviders
{
    /// <summary>
    /// A route value provider which evaluates the value from the callback.
    /// </summary>
    public sealed class CallbackRouteValueProvider : IRouteValueProvider
    {
        Func<string, RequestContext, RouteValueDictionary, object> callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackRouteValueProvider"/> class.
        /// </summary>
        /// <param name="callback">The callback to evaluate the value.</param>
        public CallbackRouteValueProvider(Func<object> callback)
            : this((key, request, values) => callback())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackRouteValueProvider"/> class.
        /// </summary>
        /// <param name="callback">The callback to evaluate the value.</param>
        public CallbackRouteValueProvider(Func<string, RequestContext, RouteValueDictionary, object> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            this.callback = callback;
        }

        /// <summary>
        /// Gets the value using the specified callback.
        /// </summary>
        /// <param name="key">The value key.</param>
        /// <param name="request">The request.</param>
        /// <param name="values">The current route values.</param>
        /// <returns>
        /// Returns the value for the specified key.
        /// </returns>
        public object GetValue(string key, RequestContext request, RouteValueDictionary values)
        {
            return callback(key, request, values);
        }
    }
}
