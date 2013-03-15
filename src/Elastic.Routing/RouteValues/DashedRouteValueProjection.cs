using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Elastic.Routing.Internals;

namespace Elastic.Routing.RouteValues
{
    /// <summary>
    /// Transforms the outgoing route value by replacing all non-word characters with dashes.
    /// </summary>
    public class DashedRouteValueProjection : IRouteValueProjection
    {
        HashSet<char> extraValidChars;
        int maxLength;
        string defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashedRouteValueProjection" /> class.
        /// </summary>
        /// <param name="allowSlash">if set to <c>true</c> the '/' characters are not replaced with dashes.</param>
        /// <param name="maxLength">The maximum length of the result.</param>
        /// <param name="defaultValue">The default value to use when the dashed value appear to be empty.</param>
        public DashedRouteValueProjection(bool allowSlash, int maxLength = 0, string defaultValue = null)
        {
            this.extraValidChars = allowSlash ? new HashSet<char> { '/' } : new HashSet<char>();
            this.maxLength = maxLength;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Does not do anything with the incoming route values.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Incoming(string key, RouteValueDictionary values)
        {
        }

        /// <summary>
        /// Dashes the outgoing route value.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Outgoing(string key, RouteValueDictionary values)
        {
            var value = (string)values[key];
            value = Utils.DashedValue(value, extraValidChars, maxLength);
            if (String.IsNullOrEmpty(value))
                value = defaultValue;
            values[key] = value;
        }
    }
}
