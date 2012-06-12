using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace Elastic.Routing.RouteValues
{
    /// <summary>
    /// A projection to replace some substring with another one when building the outgoing URL and other way around when parsing the incoming URL.
    /// </summary>
    public class ReplaceRouteValueProjection : IRouteValueProjection
    {
        /// <summary>
        /// The substring to replace when building URL or the value to replace with when parsing the URL.
        /// </summary>
        public string What { get; private set; }

        /// <summary>
        /// The string to replace with when building URL or the value to be replaced when parsing the URL.
        /// </summary>
        public string With { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceRouteValueProjection"/> class.
        /// </summary>
        /// <param name="what">The substring to replace when building URL or the value to replace with when parsing the URL.</param>
        /// <param name="with">The string to replace with when building URL or the value to be replaced when parsing the URL.</param>
        public ReplaceRouteValueProjection(string what, string with)
        {
            this.What = what;
            this.With = with;
        }

        /// <summary>
        /// Transforms the incoming route value.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Incoming(string key, RouteValueDictionary values)
        {
            values[key] = Replace(values[key], With, What);
        }

        /// <summary>
        /// Transforms the outgoing route value.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="values">The route values.</param>
        public void Outgoing(string key, RouteValueDictionary values)
        {
            values[key] = Replace(values[key], What, With);
        }

        private string Replace(object value, string pattern, string replacement)
        {
            if (value == null)
                return null;
            return Regex.Replace(value.ToString(), Regex.Escape(pattern), replacement);
        }
    }
}
