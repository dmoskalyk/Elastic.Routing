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
            requestContext = new RequestContext();
            request.SetupGet(r => r.RequestContext).Returns(requestContext);
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
