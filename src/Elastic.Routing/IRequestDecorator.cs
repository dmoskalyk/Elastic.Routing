using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Elastic.Routing
{
    /// <summary>
    /// An interface for request decorator.
    /// </summary>
    public interface IRequestDecorator
    {
        /// <summary>
        /// Decorates the specified request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        void Decorate(RequestContext requestContext);
    }
}
