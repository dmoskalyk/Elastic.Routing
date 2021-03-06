﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastic.Routing.RouteValues;
using Elastic.Routing.Constraints;

namespace Elastic.Routing
{
    /// <summary>
    /// A factory method container for route value providers.
    /// </summary>
    public static class RouteValue
    {
        /// <summary>
        /// The regex pattern matching the dashed value of at least one character with no dashes at its bounds.
        /// </summary>
        public static string DashedValuePattern = "[^-/_]([^/]*[^-/_])?";

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
        /// <param name="maxLength">The maximum length of the result.</param>
        /// <param name="defaultValue">The default value to use when the dashed value appear to be empty.</param>
        /// <returns>
        /// Returns the new instance of <see cref="DashedRouteValueProjection" /> class.
        /// </returns>
        public static DashedRouteValueProjection DashedProjection(bool allowSlash, int maxLength = 0, string defaultValue = null)
        {
            return new DashedRouteValueProjection(allowSlash, maxLength, defaultValue);
        }

        /// <summary>
        /// Creates a projection to replace some substring of the route value with another one when building the outgoing URL and other way around when parsing the incoming URL.
        /// </summary>
        /// <param name="what">The substring to replace when building URL or the value to replace with when parsing the URL.</param>
        /// <param name="with">The string to replace with when building URL or the value to be replaced when parsing the URL.</param>
        /// <returns></returns>
        public static ReplaceRouteValueProjection Replace(string what, string with)
        {
            return new ReplaceRouteValueProjection(what, with);
        }

        /// <summary>
        /// Create a constraint which requires a specific value to be present during the URL construction.
        /// </summary>
        /// <param name="value">The expected value. If not specified, only parameter existence is checked, but not the value itself.</param>
        /// <returns>Return a constaint instance.</returns>
        public static RequiredValueConstraint Required(string value = null)
        {
            return new RequiredValueConstraint(value);
        }
    }
}
