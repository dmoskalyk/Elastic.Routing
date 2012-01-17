using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using System.Web;
using Moq;
using System.Collections.Specialized;
using System.Data;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class UrlMatchingTests : ElasticRouteTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            InitializeBase();
        }

        [TestMethod]
        [DeploymentItem("Elastic.Routing.Tests\\DataSource_Common.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\DataSource_Common.xml", 
            "Row", DataAccessMethod.Sequential)]
        public void ElasticRoute_GetRouteData_CommonDataSource()
        {
            RunTest();
        }

        [TestMethod]
        [DeploymentItem("Elastic.Routing.Tests\\DataSource_Incoming.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\DataSource_Incoming.xml",
            "Row", DataAccessMethod.Sequential)]
        public void ElasticRoute_GetRouteData_SpecificDataSource()
        {
            RunTest();
        }

        private void RunTest()
        {
            var @params = GetParams();

            var route = new ElasticRoute(@params.Pattern,
                routeHandler: routeHandler,
                constraints: @params.Constraints,
                incomingDefaults: @params.Defaults);
            request.SetupGet(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/" + @params.Url);
            var data = route.GetRouteData(context.Object);
            if (!@params.Result)
            {
                Assert.IsNull(data);
            }
            else
            {
                Assert.IsNotNull(data);

                foreach (var entry in @params.RouteValues)
                {
                    var strValue = (string)entry.Value;
                    var actual = (string)data.Values[entry.Key];
                    if (strValue == string.Empty)
                        Assert.IsNull(actual);
                    else
                        Assert.AreEqual(strValue, actual);
                }
            }
        }
    }
}
