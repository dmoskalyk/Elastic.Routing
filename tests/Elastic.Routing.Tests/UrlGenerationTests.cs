using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using Moq;
using System.Web;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class UrlGenerationTests : ElasticRouteTestBase
    {
        RequestContext requestContext;

        [TestInitialize]
        public void Initialize()
        {
            InitializeBase();
            requestContext = new RequestContext() { HttpContext = context.Object };
            request.SetupGet(r => r.RequestContext).Returns(requestContext);
        }

        [TestMethod]
        public void ElasticRoute_GetVirtualPath_MatchConstraints()
        {
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("url1", routeHandler: routeHandler,
                    constraints: new { controller = "Controller1", action = "Action1" },
                    outgoingDefaults: new { controller = "Controller1", action = "Action1" }),
                new ElasticRoute("url2", routeHandler: routeHandler,
                    constraints: new { controller = "Controller2", action = "Action1" },
                    outgoingDefaults: new { controller = "Controller2", action = "Action1" }),
                new ElasticRoute("url3", routeHandler: routeHandler,
                    constraints: new { controller = "Controller2", action = "Action2" },
                    outgoingDefaults: new { controller = "Controller2", action = "Action2" })
            };

            var routeValues = new RouteValueDictionary(new { controller = "Controller2", action = "Action2" });
            var virtualPath = routeCollection.GetVirtualPath(requestContext, routeValues);
            Assert.AreEqual("/url3", virtualPath.VirtualPath);
        }

        [TestMethod]
        public void ElasticRoute_GetVirtualPath_DefaultValueIsNotAdded()
        {
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("{controller}/({action})", routeHandler: routeHandler,
                    constraints: new { controller = "Controller1", action = "Action1" },
                    outgoingDefaults: new { controller = "Controller1", action = "Action1" })
            };

            var routeValues = new RouteValueDictionary(new { controller = "Controller1", action = "Action1" });
            var virtualPath = routeCollection.GetVirtualPath(requestContext, routeValues);
            Assert.AreEqual("/controller1/", virtualPath.VirtualPath);
        }

        [TestMethod]
        public void ElasticRoute_GetVirtualPath_DiacriticsInQueryString()
        {
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("search", routeHandler: routeHandler)
            };

            var routeValues = new RouteValueDictionary(new { q = "ball ø" });
            var virtualPath = routeCollection.GetVirtualPath(requestContext, routeValues);
            Assert.AreEqual("/search?q=ball+%c3%b8", virtualPath.VirtualPath);
        }

        [TestMethod]
        [DeploymentItem("Elastic.Routing.Tests\\DataSource_Common.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\DataSource_Common.xml",
            "Row", DataAccessMethod.Sequential)]
        public void ElasticRoute_GetVirtualPath_CommonDataSource()
        {
            RunTest();
        }

        [TestMethod]
        [DeploymentItem("Elastic.Routing.Tests\\DataSource_Outgoing.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\DataSource_Outgoing.xml",
            "Row", DataAccessMethod.Sequential)]
        public void ElasticRoute_GetVirtualPath_SpecificDataSource()
        {
            RunTest();
        }

        private void RunTest()
        {
            var @params = GetParams();

            var route = new ElasticRoute(@params.Pattern,
                routeHandler: routeHandler,
                constraints: @params.Constraints,
                outgoingDefaults: @params.Defaults);

            var path = route.GetVirtualPath(requestContext, @params.RouteValues);
            if (@params.Result)
            {
                Assert.IsNotNull(path);
                Assert.AreEqual(@params.Url, path.VirtualPath);
            }
            else
            {
                Assert.IsNull(path);
            }
        }
    }
}
