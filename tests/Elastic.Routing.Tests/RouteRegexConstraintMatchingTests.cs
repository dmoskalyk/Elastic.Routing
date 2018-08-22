using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using System.Threading;
using Moq;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class RouteRegexConstraintMatchingTests : ElasticRouteTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            InitializeBase();
            request.SetupGet(c => c.RequestContext).Returns(() => new Mock<RequestContext>().Object);
        }

        const string fourLettersLanguage = "([a-zA-Z]{2})(-([a-zA-Z]{2}))?";

        [TestMethod]
        public void ElasticRoute_GetRouteData_OptionalRegexParameter_IsAddedToRouteValues()
        {
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("({lang}/)({groupId}/){id}",
                    routeHandler: routeHandler,
                    constraints: new { lang = new TestRegexConstraint(fourLettersLanguage) }
                    )
            };

            request.SetupGet(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/en-us/123");
            var routeData = routeCollection.GetRouteData(context.Object);
            Assert.IsNotNull(routeData);
            Assert.AreEqual(2, routeData.Values.Count);
            Assert.AreEqual("en-us", routeData.Values["lang"]);
            Assert.AreEqual("123", routeData.Values["id"]);
        }

        [TestMethod]
        public void ElasticRoute_GetRouteData_OptionalRegexParameter_IsNotAddedToRouteValues()
        {
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("({lang}/)({groupId}/){id}",
                    routeHandler: routeHandler,
                    constraints: new { lang = new TestRegexConstraint(fourLettersLanguage) }
                    )
            };

            request.SetupGet(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/t-est/123");
            var routeData = routeCollection.GetRouteData(context.Object);

            Assert.IsNotNull(routeData);
            Assert.AreEqual(2, routeData.Values.Count);
            Assert.AreEqual("t-est", routeData.Values["groupId"]);
            Assert.AreEqual("123", routeData.Values["id"]);
        }

        [TestMethod]
        public void ElasticRoute_GetRouteData_OptionalDynamicRegexParameter_AddedToRouteValuesAfterUpdate()
        {
            var constraint = new TestRegexConstraint(fourLettersLanguage);
            var routeCollection = new RouteCollection()
            {
                new ElasticRoute("({lang}/)({groupId}/){id}",
                    routeHandler: routeHandler,
                    constraints: new { lang = constraint }
                    )
            };
            request.SetupGet(r => r.AppRelativeCurrentExecutionFilePath).Returns("~/kok/123");

            var routeData = routeCollection.GetRouteData(context.Object);

            Assert.IsNotNull(routeData);
            Assert.AreEqual(2, routeData.Values.Count);
            Assert.AreEqual("kok", routeData.Values["groupId"]);
            Assert.AreEqual("123", routeData.Values["id"]);

            constraint.UpdatePattern("([a-zA-Z]{2}|kok)(-([a-zA-Z]{2}))?");
            routeData = routeCollection.GetRouteData(context.Object);

            Assert.IsNotNull(routeData);
            Assert.AreEqual(2, routeData.Values.Count);
            Assert.AreEqual("kok", routeData.Values["lang"]);
            Assert.AreEqual("123", routeData.Values["id"]);
        }
    }
}
