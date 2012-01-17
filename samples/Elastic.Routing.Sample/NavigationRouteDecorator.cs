using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Elastic.Routing.Sample
{
    public class NavigationRouteDecorator : IRequestDecorator
    {
        public void Decorate(RequestContext requestContext)
        {
            Process(requestContext.RouteData.Values);
        }

        private void Process(RouteValueDictionary values)
        {
            var nodeId = (string)values["id"];
            var actionInfo = GetActionForNode(nodeId);
            values["controller"] = actionInfo.Item1;
            values["action"] = actionInfo.Item2;
        }

        private Tuple<string, string> GetActionForNode(string id)
        {
            // resolving navigation node type
            if (id == "1")
                return new Tuple<string, string>("Elastic", "node1");
            else
                return new Tuple<string, string>("Elastic", "NotFound");
        }
    }
}