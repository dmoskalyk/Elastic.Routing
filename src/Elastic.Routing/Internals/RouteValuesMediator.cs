using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace Elastic.Routing.Internals
{
    /// <summary>
    /// A mediator for route values access which retrieves, sets and validates them.
    /// </summary>
    public class RouteValuesMediator
    {
        /// <summary>
        /// The route wrapper to be passed to the custom route constraints.
        /// </summary>
        protected RouteWrapper route;

        /// <summary>
        /// The request context.
        /// </summary>
        protected RequestContext request;

        /// <summary>
        /// Current route values.
        /// </summary>
        protected RouteValueDictionary values;

        /// <summary>
        /// Default route values.
        /// </summary>
        protected RouteValueDictionary defaults;

        /// <summary>
        /// Route values constraints.
        /// </summary>
        protected RouteValueDictionary constraints;

        /// <summary>
        /// Current route direction.
        /// </summary>
        protected RouteDirection routeDirection;

        /// <summary>
        /// Gets the list of all route values keys which did not pass constraints validation since this instance was created.
        /// </summary>
        public HashSet<string> InvalidatedKeys { get; private set; }

        /// <summary>
        /// Gets the list of all route values keys which were resolved or set from the outside since this instance was created.
        /// </summary>
        public HashSet<string> VisitedKeys { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteValuesMediator"/> class.
        /// </summary>
        /// <param name="route">The route wrapper.</param>
        /// <param name="request">The request context.</param>
        /// <param name="values">The current route values.</param>
        /// <param name="defaults">The default route values.</param>
        /// <param name="constraints">The route values constraints.</param>
        /// <param name="routeDirection">The route direction.</param>
        public RouteValuesMediator(RouteWrapper route, RequestContext request, RouteValueDictionary values, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteDirection routeDirection)
        {
            this.route = route;
            this.request = request;
            this.values = values;
            this.defaults = defaults;
            this.constraints = constraints;
            this.routeDirection = routeDirection;
            this.InvalidatedKeys = new HashSet<string>();
            this.VisitedKeys = new HashSet<string>();
        }

        /// <summary>
        /// Resolves the route value by the key. If the value does not match the route constraint, <c>null</c> is returned.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the current route value or default if available.</returns>
        public virtual string ResolveValue(string key)
        {
            VisitedKeys.Add(key);
            var value = values[key];
            if (value != null && !CheckCustomConstraint(key))
                return null;

            value = value ?? GetDefaultValue(key);
            if (value == null)
                return null;
            else
                return value.ToString();
        }

        /// <summary>
        /// Sets the route value for the specified key with custom constraint validation.
        /// </summary>
        /// <param name="key">The route value key.</param>
        /// <param name="value">The value.</param>
        public virtual void SetValue(string key, object value)
        {
            VisitedKeys.Add(key);
            values[key] = value ?? GetDefaultValue(key);
            CheckCustomConstraint(key);
        }

        /// <summary>
        /// Matches all constraints against the current route values.
        /// </summary>
        /// <returns>The value indicating whether all constraints match or not.</returns>
        public virtual bool MatchConstraints(HashSet<string> requiredParameters)
        {
            foreach (var constraint in constraints)
            {
                var objValue = values[constraint.Key];
                if (objValue == null && !requiredParameters.Contains(constraint.Key))
                    continue;
                var value = (objValue ?? string.Empty).ToString();
                if (constraint.Value is string)
                {
                    if (!Regex.IsMatch(value, (string)constraint.Value, RegexOptions.IgnoreCase))
                        return false;
                }
                else
                {
                    if (!CheckCustomConstraint(constraint.Key))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the default route value for the specified key. If the default value is of type <see cref="IRouteValueProvider"/>, then the value is retrieved using the specified provider.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the default route value by the key.</returns>
        protected virtual object GetDefaultValue(string key)
        {
            return Utils.EvaluateDefaultValue(request, defaults, key);
        }

        /// <summary>
        /// Evaluates the custom route constraint for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the value indicating if the validation succeeded.</returns>
        protected virtual bool CheckCustomConstraint(string key)
        {
            var constraint = constraints[key] as IRouteConstraint;
            if (constraint != null && !constraint.Match(request.HttpContext, route, key, values, routeDirection))
            {
                InvalidatedKeys.Add(key);
                return false;
            }
            return true;
        }
    }
}
