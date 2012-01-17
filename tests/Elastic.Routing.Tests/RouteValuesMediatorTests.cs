using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Routing;
using Moq;
using System.Web;
using Elastic.Routing.Internals;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class RouteValuesMediatorTests
    {
        Mock<RequestContext> request;

        [TestInitialize]
        public void Initialize()
        {
            request = new Mock<RequestContext>();
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_NoValue()
        {
            var mediator = Create(RouteDirection.IncomingRequest);
            var actual = mediator.ResolveValue("key1");
            Assert.IsNull(actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromValues()
        {
            var key1 = "value";
            var mediator = Create(RouteDirection.IncomingRequest, values: new { key1 });
            var actual = mediator.ResolveValue("key1");
            Assert.AreEqual(key1, actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromDefaults()
        {
            var key1 = "default value";
            var mediator = Create(RouteDirection.IncomingRequest, defaults: new { key1 });
            var actual = mediator.ResolveValue("key1");
            Assert.AreEqual(key1, actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromDefaults_ValueProvider()
        {
            var key1 = "provided value";
            var valueProvider = new Mock<IRouteValueProvider>();
            valueProvider.Setup(p => p.GetValue("key1", It.IsAny<RequestContext>(), It.IsAny<RouteValueDictionary>())).Returns(key1);
            var mediator = Create(RouteDirection.IncomingRequest, defaults: new { key1 = valueProvider.Object });
            var actual = mediator.ResolveValue("key1");
            Assert.AreEqual(key1, actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromValues_Valid()
        {
            var key1 = "value";
            var mediator = Create(RouteDirection.IncomingRequest, 
                values: new { key1 }, 
                constraints: new { key1 = CreateConstraint("key1", true) });
            var actual = mediator.ResolveValue("key1");
            Assert.AreEqual(key1, actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromValues_NotValid()
        {
            var key1 = "value";
            var mediator = Create(RouteDirection.IncomingRequest, 
                values: new { key1 }, 
                constraints: new { key1 = CreateConstraint("key1", false) });
            var actual = mediator.ResolveValue("key1");
            Assert.IsNull(actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsTrue(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_ResolveValue_FromDefaults_NoValidation()
        {
            var key1 = "default value";
            var mediator = Create(RouteDirection.IncomingRequest,
                defaults: new { key1 },
                constraints: new { key1 = CreateConstraint("key1", false) });
            var actual = mediator.ResolveValue("key1");
            Assert.AreEqual(key1, actual);
            Assert.IsTrue(mediator.VisitedKeys.Contains("key1"));
            Assert.IsFalse(mediator.InvalidatedKeys.Contains("key1"));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_Null_NoDefaults()
        {
            var key = "key1";
            var values = new RouteValueDictionary();
            var mediator = Create(RouteDirection.UrlGeneration, values: values);
            mediator.SetValue(key, null);
            Assert.AreEqual(1, values.Count);
            Assert.IsNull(values[key]);
            Assert.IsFalse(mediator.InvalidatedKeys.Contains(key));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_Null_FromDefaults()
        {
            var key = "key1";
            var value = "default value";
            var values = new RouteValueDictionary();
            var mediator = Create(RouteDirection.UrlGeneration, values: values, defaults: new { key1 = value });
            mediator.SetValue(key, null);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(value, values[key]);
            Assert.IsFalse(mediator.InvalidatedKeys.Contains(key));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_Null_FromDefaults_ValueProvider()
        {
            var key = "key1";
            var value = "default value";
            var values = new RouteValueDictionary();
            var valueProvider = new Mock<IRouteValueProvider>();
            valueProvider.Setup(p => p.GetValue(key, It.IsAny<RequestContext>(), It.IsAny<RouteValueDictionary>())).Returns(value);
            var mediator = Create(RouteDirection.UrlGeneration, values: values, defaults: new { key1 = valueProvider.Object });
            mediator.SetValue(key, null);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(value, values[key]);
            Assert.IsFalse(mediator.InvalidatedKeys.Contains(key));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_NotNull_NoConstraint()
        {
            var key = "key1";
            var value = "value1";
            var values = new RouteValueDictionary();
            var mediator = Create(RouteDirection.UrlGeneration, values: values);
            mediator.SetValue(key, value);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(value, values[key]);
            Assert.IsFalse(mediator.InvalidatedKeys.Contains(key));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_NotNull_NotValid()
        {
            var key = "key1";
            var value = "value1";
            var values = new RouteValueDictionary();
            var mediator = Create(RouteDirection.UrlGeneration, values: values, constraints: new { key1 = CreateConstraint(key, false) });
            mediator.SetValue(key, value);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(value, values[key]);
            Assert.IsTrue(mediator.InvalidatedKeys.Contains(key));
        }

        [TestMethod]
        public void RouteValuesMediator_SetValue_NotNull_Valid()
        {
            var key = "key1";
            var value = "value1";
            var values = new RouteValueDictionary();
            var mediator = Create(RouteDirection.UrlGeneration, values: values, constraints: new { key1 = CreateConstraint(key, true) });
            mediator.SetValue(key, value);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual(value, values[key]);
            Assert.IsFalse(mediator.InvalidatedKeys.Contains(key));
        }

        private RouteValuesMediator Create(RouteDirection routeDirection, object values = null, object defaults = null, object constraints = null)
        {
            return new RouteValuesMediator(null, request.Object, ToDic(values), ToDic(defaults), ToDic(constraints), routeDirection);
        }

        private RouteValueDictionary ToDic(object values)
        {
            if (values is RouteValueDictionary)
                return (RouteValueDictionary)values;
            return values != null ? new RouteValueDictionary(values) : new RouteValueDictionary();
        }

        private IRouteConstraint CreateConstraint(string parameterName, bool matchResult)
        {
            var constraint = new Mock<IRouteConstraint>();
            constraint.Setup(c => 
                c.Match(
                    It.IsAny<HttpContextBase>(), 
                    It.IsAny<Route>(), 
                    parameterName, 
                    It.IsAny<RouteValueDictionary>(), 
                    It.IsAny<RouteDirection>())
                    ).Returns(matchResult);
            return constraint.Object;
        }
    }
}
