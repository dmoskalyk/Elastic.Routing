using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Moq;
using System.Web.Routing;
using System.Data;

namespace Elastic.Routing.Tests
{
    public abstract class ElasticRouteTestBase
    {
        protected IRouteHandler routeHandler;
        protected Mock<HttpContextBase> context;
        protected Mock<HttpRequestBase> request;

        public TestContext TestContext { get; set; }

        protected void InitializeBase()
        {
            routeHandler = new PageRouteHandler("~/");
            context = new Mock<HttpContextBase>();
            request = new Mock<HttpRequestBase>();
            context.SetupGet(c => c.Request).Returns(() => request.Object);
        }

        protected TestParams GetParams()
        {
            var result = new TestParams();
            var row = TestContext.DataRow;
            if (row != null)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    var key = column.ColumnName.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                    var objValue = row[column.ColumnName];
                    var value = objValue is DBNull ? null : (string)objValue;
                    if (key.Length == 1)
                    {
                        if (key[0] == "Pattern")
                            result.Pattern = value;
                        else if (key[0] == "Url")
                            result.Url = value;
                        else if (key[0] == "Result")
                            result.Result = bool.Parse(value);
                        else
                            result.RouteValues[key[0]] = value;
                    }
                    else if (key[0] == "default")
                        result.Defaults[key[1]] = value;
                    else if (key[0] == "constraint")
                        result.Constraints[key[1]] = value;
                }
            }
            return result;
        }

        protected class TestParams
        {
            public string Pattern { get; set; }
            public string Url { get; set; }
            public bool Result { get; set; }
            public RouteValueDictionary Constraints { get; private set; }
            public RouteValueDictionary Defaults { get; private set; }

            public RouteValueDictionary RouteValues { get; private set; }

            public TestParams()
            {
                this.Constraints = new RouteValueDictionary();
                this.Defaults = new RouteValueDictionary();
                this.RouteValues = new RouteValueDictionary();
            }
        }

    }
}
