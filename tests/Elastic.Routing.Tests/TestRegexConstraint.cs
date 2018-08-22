using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Elastic.Routing.Tests
{
    public class TestRegexConstraint : IDynamicRegexRouteConstraint
    {
        public TestRegexConstraint(string pattern)
        {
            Pattern = pattern;
        }

        public void UpdatePattern(string newPattern)
        {
            Pattern = newPattern;
            RegexChanged?.Invoke(this, EventArgs.Empty);
        }

        public string Pattern { get; private set; }

        public event EventHandler RegexChanged;

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            => true;
    }
}
