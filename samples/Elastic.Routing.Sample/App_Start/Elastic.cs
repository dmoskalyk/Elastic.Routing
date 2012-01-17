using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Threading;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Elastic.Routing.Sample.App_Start.Elastic), "Start")]

namespace Elastic.Routing.Sample.App_Start
{
    public static class Elastic
    {
        public static void Start()
        {
            var routes = RouteTable.Routes;

            routes.Map("({lang}/)({controller}/){action}/", new MvcRouteHandler(),
                incomingDefaults: new
                {
                    lang = RouteValue.Dynamic(() => Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()),
                    controller = "Elastic"
                },
                outgoingDefaults: new
                {
                    lang = RouteValue.FromRequest()
                },
                constraints: new
                {
                    lang = @"\w{2}-\w{2}",
                    action = @"\w+"
                }
                );

            routes.Map("({lang}/){*path}/{id}/({title}(.{format}))",
                new DecoratedRouteHandler(new MvcRouteHandler(), new NavigationRouteDecorator()),
                constraints: new
                {
                    lang = @"\w{2}-\w{2}",
                    id = @"\d+",
                    title = @"[^\./]+",
                    format = new Constraints.DelegatedConstraint(v => v == null || v == "json" || v == "xml" || v == "html")
                },
                incomingDefaults: new
                {
                    format = "html"
                }
            );

            routes.Map("({lang}/)", new MvcRouteHandler(),
                constraints: new
                {
                    lang = @"\w{2}-\w{2}"
                },
                incomingDefaults: new
                {
                    controller = "Elastic",
                    action = "page"
                },
                routeName: "Homepage"
            );
        }
    }
}