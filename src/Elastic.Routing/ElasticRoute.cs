using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web;
using Elastic.Routing.Parsing;
using Elastic.Routing.Internals;

namespace Elastic.Routing
{
    /// <summary>
    /// An featured route with the support of wildcard parameters, optional parts etc.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Url: {Url}, Handler: {RouteHandler.GetType().Name}")]
    public class ElasticRoute : RouteBase
    {
        /// <summary>
        /// The list of system route value keys which are used in URL construction to be not included in the query string and are always checked for being matched by their constraints (if any).
        /// By default, they are "controller", "action" and "route".
        /// </summary>
        protected static HashSet<string> systemRouteValueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "controller", "action", "route" };

        private static RouteValueDictionary EmptyValues()
        {
            return new RouteValueDictionary();
        }

        private Regex urlMatch;
        private FullPathSegment fullPathSegment;

        /// <summary>
        /// A route wrapper to be passed to the custom route constraints.
        /// </summary>
        protected RouteWrapper routeWrapper;

        /// <summary>
        /// Gets the URL pattern of the current route.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets the route handler.
        /// </summary>
        public IRouteHandler RouteHandler { get; private set; }

        /// <summary>
        /// Gets the default route values used for parsing an incoming URL.
        /// </summary>
        public RouteValueDictionary IncomingDefaults { get; private set; }

        /// <summary>
        /// Gets the default route values used in URL generation.
        /// </summary>
        public RouteValueDictionary OutgoingDefaults { get; private set; }

        /// <summary>
        /// Gets the route constraints.
        /// </summary>
        public RouteValueDictionary Constraints { get; private set; }

        /// <summary>
        /// Gets the route values projections.
        /// </summary>
        public IDictionary<string, IRouteValueProjection> Projections { get; private set; }

        /// <summary>
        /// Gets or sets the data tokens.
        /// </summary>
        /// <value>
        /// The data tokens.
        /// </value>
        public RouteValueDictionary DataTokens { get; set; }

        /// <summary>
        /// Gets the required parameters names.
        /// </summary>
        /// <value>
        /// The required parameters names.
        /// </value>
        public HashSet<string> RequiredParameters { get; private set; }

        /// <summary>
        /// Gets the required parameters names.
        /// </summary>
        /// <value>
        /// The required parameters names.
        /// </value>
        public HashSet<string> AllParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticRoute"/> class.
        /// </summary>
        /// <param name="url">The URL pattern.</param>
        /// <param name="routeHandler">The route handler.</param>
        public ElasticRoute(string url, IRouteHandler routeHandler)
            : this(url, routeHandler, EmptyValues(), null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticRoute"/> class.
        /// </summary>
        /// <param name="url">The URL pattern.</param>
        /// <param name="routeHandler">The route handler.</param>
        /// <param name="constraints">The route constraints.</param>
        /// <param name="incomingDefaults">The incoming request default route values.</param>
        /// <param name="outgoingDefaults">The URL generation default route values.</param>
        /// <param name="projections">The route values projections.</param>
        /// <param name="dataTokens">The data tokens.</param>
        public ElasticRoute(string url, IRouteHandler routeHandler,
            object constraints = null,
            object incomingDefaults = null,
            object outgoingDefaults = null,
            object projections = null,
            object dataTokens = null)
            : this(url, routeHandler,
                constraints: constraints.ToRouteValues(),
                incomingDefaults: incomingDefaults.ToRouteValues(),
                outgoingDefaults: outgoingDefaults.ToRouteValues(),
                projections: projections.ToRouteValues().Cast<IRouteValueProjection>(),
                dataTokens: dataTokens.ToRouteValues())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticRoute"/> class.
        /// </summary>
        /// <param name="url">The URL pattern.</param>
        /// <param name="routeHandler">The route handler.</param>
        /// <param name="constraints">The route constraints.</param>
        /// <param name="incomingDefaults">The incoming request default route values.</param>
        /// <param name="outgoingDefaults">The URL generation default route values.</param>
        /// <param name="projections">The route values projections.</param>
        /// <param name="dataTokens">The data tokens.</param>
        public ElasticRoute(string url, IRouteHandler routeHandler,
            RouteValueDictionary constraints = null,
            RouteValueDictionary incomingDefaults = null,
            RouteValueDictionary outgoingDefaults = null,
            IDictionary<string, IRouteValueProjection> projections = null,
            RouteValueDictionary dataTokens = null)
        {
            this.Url = url;
            this.RouteHandler = routeHandler;
            this.IncomingDefaults = incomingDefaults ?? EmptyValues();
            this.OutgoingDefaults = outgoingDefaults ?? EmptyValues();
            this.Constraints = constraints ?? EmptyValues();
            this.Projections = projections ?? new Dictionary<string, IRouteValueProjection>();
            this.DataTokens = dataTokens ?? EmptyValues();

            this.routeWrapper = new RouteWrapper(this);
            Reinitialize();
        }

        /// <summary>
        /// Parses the URL pattern to reflect new route constraints.
        /// </summary>
        public void Reinitialize()
        {
            this.fullPathSegment = ParseSegments(this.Url);

            this.RequiredParameters = new HashSet<string>(fullPathSegment.RequiredParameters.Select(p => p.Name), StringComparer.InvariantCultureIgnoreCase);
            this.AllParameters = new HashSet<string>(fullPathSegment.Parameters, StringComparer.InvariantCultureIgnoreCase);

            this.urlMatch = BuildRegex(fullPathSegment);
        }

        /// <summary>
        /// When overridden in a derived class, returns route information about the request.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition if the route matches the current request, or null if the route does not match the request.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var path = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            var routeData = GetRouteData(httpContext, path.ToLowerInvariant());
            return routeData;
        }

        /// <summary>
        /// Gets the route data for the specified url path.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="path">The path.</param>
        /// <returns>
        /// An object that contains the values from the route definition if the route matches the current request, or null if the route does not match the request.
        /// </returns>
        protected virtual RouteData GetRouteData(HttpContextBase httpContext, string path)
        {
            var match = urlMatch.Match(path);
            if (!match.Success)
                return null;

            var data = new RouteData(this, RouteHandler);
            var valuesMediator = CreateMediator(httpContext.Request.RequestContext, data.Values, RouteDirection.IncomingRequest);
            ExtractRouteValues(fullPathSegment, match, valuesMediator);
            if (valuesMediator.InvalidatedKeys.Count > 0)
                return null;

            foreach (var defaultValue in this.IncomingDefaults)
            {
                if (valuesMediator.VisitedKeys.Contains(defaultValue.Key))
                    continue;

                data.Values[defaultValue.Key] = (string)valuesMediator.ResolveValue(defaultValue.Key);
            }

            foreach (var projection in Projections)
            {
                if (projection.Value == null)
                    continue;
                projection.Value.Incoming(projection.Key, data.Values);
            }

            this.DataTokens.CopyTo(data.DataTokens);

            return data;
        }

        /// <summary>
        /// When overridden in a derived class, checks whether the route matches the specified values, and if so, generates a URL and retrieves information about the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>
        /// An object that contains the generated URL and information about the route, or null if the route does not match <paramref name="values"/>.
        /// </returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            var routeValues = new RouteValueDictionary(values);
            foreach (var projection in Projections)
            {
                if (projection.Value == null)
                    continue;
                projection.Value.Outgoing(projection.Key, routeValues);
            }

            var valuesMediator = CreateMediator(requestContext, routeValues, RouteDirection.UrlGeneration);
            var parametersToCheck = fullPathSegment.RequiredParameters.Select(p => p.Name).Concat(systemRouteValueKeys);
            if (!valuesMediator.MatchConstraints(new HashSet<string>(parametersToCheck)))
                return null;

            var url = ConstructUrl(fullPathSegment, valuesMediator);
            if (url == null || !urlMatch.IsMatch(url))
                return null;

            var queryString = BuildQueryString(valuesMediator, routeValues);
            if (queryString != null)
                url += '?' + queryString;

            var data = new VirtualPathData(this, url.ToLowerInvariant());
            this.DataTokens.CopyTo(data.DataTokens);
            return data;
        }

        /// <summary>
        /// Creates the mediator used for internal access to route values.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="values">The current route values.</param>
        /// <param name="routeDirection">The route direction.</param>
        /// <returns>Returns a newly created and initialized instance of <see cref="RouteValuesMediator"/> class.</returns>
        protected virtual RouteValuesMediator CreateMediator(RequestContext requestContext, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var defaults = routeDirection == RouteDirection.IncomingRequest ? IncomingDefaults : OutgoingDefaults;
            return new RouteValuesMediator(routeWrapper, requestContext, values, defaults, Constraints, routeDirection);
        }

        /// <summary>
        /// Parses the URL pattern into a segments tree.
        /// </summary>
        /// <param name="urlPattern">The URL pattern.</param>
        /// <returns>Returns the parsed URL segments tree.</returns>
        protected virtual FullPathSegment ParseSegments(string urlPattern)
        {
            return PathSegmentParser.Instance.Parse(urlPattern, Constraints);
        }

        /// <summary>
        /// Builds the <see cref="Regex"/> instance based on the specified <paramref name="path"/> for matching the concrete URLs.
        /// </summary>
        /// <param name="path">The parsed path segments tree.</param>
        /// <returns>Returns the <see cref="Regex"/> instance used to match the concrete URLs.</returns>
        protected virtual Regex BuildRegex(FullPathSegment path)
        {
            var options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;
            return new Regex(path.GetRegexPattern(), options);
        }

        /// <summary>
        /// Extracts the route values for the specified <paramref name="path"/> from the <paramref name="match"/>.
        /// </summary>
        /// <param name="path">The path segments tree.</param>
        /// <param name="match">The successful match result to extract values from.</param>
        /// <param name="valuesMediator">The route values mediator.</param>
        protected virtual void ExtractRouteValues(FullPathSegment path, Match match, RouteValuesMediator valuesMediator)
        {
            path.ExtractRouteValues(match, valuesMediator.SetValue);
        }

        /// <summary>
        /// Constructs the URL for the specified <paramref name="path"/> using the <paramref name="valuesMediator"/>.
        /// </summary>
        /// <param name="path">The path segments tree.</param>
        /// <param name="valuesMediator">The route values mediator.</param>
        /// <returns>Returns the constructed URL or <c>null</c> if the URL cannot be constructed.</returns>
        protected virtual string ConstructUrl(FullPathSegment path, RouteValuesMediator valuesMediator)
        {
            return (string)path.GetUrlPart((s) => valuesMediator.ResolveValue(s));
        }

        /// <summary>
        /// Builds the query string based on the extra route values which were passed explicitly but not used in URL construction.
        /// </summary>
        /// <param name="valuesMediator">The route values mediator used for URL construction.</param>
        /// <param name="values">The current route values.</param>
        /// <returns>Returns the query string or <c>null</c> when it cannot be built.</returns>
        protected virtual string BuildQueryString(RouteValuesMediator valuesMediator, RouteValueDictionary values)
        {
            var queryString = new List<KeyValuePair<string, object>>();
            foreach (var entry in values)
            {
                if (systemRouteValueKeys.Contains(entry.Key) ||
                    valuesMediator.VisitedKeys.Contains(entry.Key) ||
                    valuesMediator.InvalidatedKeys.Contains(entry.Key) ||
                    entry.Value == null ||
                    OutgoingDefaults.HasValue(entry.Key, entry.Value) ||
                    IncomingDefaults.HasValue(entry.Key, entry.Value))
                    continue;

                queryString.Add(entry);
            }

            if (queryString.Count == 0)
                return null;

            var pairs = queryString.Select(e => HttpUtility.UrlEncode(e.Key) + "=" + HttpUtility.UrlEncode(e.Value.ToString()));
            return string.Join("&", pairs);
        }
    }
}
