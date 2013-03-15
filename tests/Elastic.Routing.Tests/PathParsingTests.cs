using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Elastic.Routing.Parsing;
using System.Web.Routing;

namespace Elastic.Routing.Tests
{
    [TestClass]
    public class PathParsingTests
    {
        RouteValueDictionary constraints;
        PathSegmentParser parser;

        [TestInitialize]
        public void Initialize()
        {
            parser = PathSegmentParser.Instance;
            constraints = new RouteValueDictionary();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PathSegmentParser_NullUrl()
        {
            string url = null;
            var path = parser.Parse(url, constraints);
        }

        [TestMethod]
        public void PathSegmentParser_EmptyUrl()
        {
            string url = string.Empty;
            var path = parser.Parse(url, constraints);
            Assert.IsNotNull(path);
            Assert.AreEqual(0, path.Segments.Count);
        }

        [TestMethod]
        public void PathSegmentParser_InvalidParameterSyntax()
        {
            string url = "w/{title.aspx";
            var path = parser.Parse(url, constraints);
            Assert.IsNotNull(path);
            Assert.AreEqual(1, path.Segments.Count);
            Assert_Literal(path.Segments[0], url);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void PathSegmentParser_DuplicateParameters()
        {
            string url = "{title}/node/{title}.aspx";
            var path = parser.Parse(url, constraints);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void PathSegmentParser_ExtraOpeningBrace()
        {
            string url = "{path}(/node)/(page.aspx";
            var path = parser.Parse(url, constraints);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void PathSegmentParser_ExtraClosingBrace()
        {
            string url = "{path}(/node)/page).aspx";
            var path = parser.Parse(url, constraints);
        }

        [TestMethod]
        public void PathSegmentParser_CorrectUrl_NoConstraints()
        {
            string url = "({lang}/){*path}/test/({controller}(/{action}))";
            var path = parser.Parse(url, constraints);
            Assert.AreEqual(4, path.Segments.Count);
            Assert_Optional(path.Segments[0], 2);
            Assert_Parameter(path.Segments[0].Segments[0], "lang");
            Assert_Literal(path.Segments[0].Segments[1], "/");
            Assert_Parameter(path.Segments[1], "path");
            Assert.IsInstanceOfType(path.Segments[1], typeof(WildcardPathSegment));
            Assert_Literal(path.Segments[2], "/test/");
            Assert_Optional(path.Segments[3], 2);
            Assert_Parameter(path.Segments[3].Segments[0], "controller");
            Assert_Optional(path.Segments[3].Segments[1], 2);
            Assert_Literal(path.Segments[3].Segments[1].Segments[0], "/");
            Assert_Parameter(path.Segments[3].Segments[1].Segments[1], "action");
        }

        private void Assert_Parameter(PathSegment segment, string expectedName)
        {
            Assert.IsInstanceOfType(segment, typeof(ParameterPathSegment));
            Assert.AreEqual(expectedName, ((ParameterPathSegment)segment).Name);
        }

        private void Assert_Literal(PathSegment segment, string expectedText)
        {
            Assert.IsInstanceOfType(segment, typeof(LiteralPathSegment));
            Assert.AreEqual(expectedText, ((LiteralPathSegment)segment).Text);
        }

        private void Assert_Optional(PathSegment segment, int expectedChildCount)
        {
            Assert.IsInstanceOfType(segment, typeof(OptionalPathSegment));
            Assert.AreEqual(expectedChildCount, segment.Segments.Count);
        }
    }
}
