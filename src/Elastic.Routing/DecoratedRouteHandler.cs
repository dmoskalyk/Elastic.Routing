using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web;

namespace Elastic.Routing
{
    /// <summary>
    /// A route handler decorates the request context before passing the execution to the inner handler.
    /// </summary>
    public class DecoratedRouteHandler : IRouteHandler
    {
        IRouteHandler innerHandler;
        IList<IRequestDecorator> decorators;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedRouteHandler"/> class.
        /// </summary>
        /// <param name="innerHandler">The route handler which handles all requests.</param>
        /// <param name="decorators">The request decorators.</param>
        public DecoratedRouteHandler(IRouteHandler innerHandler, params IRequestDecorator[] decorators)
        {
            this.innerHandler = innerHandler;
            this.decorators = decorators;
        }

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>
        /// An object that processes the request.
        /// </returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            Decorate(requestContext);
            return innerHandler.GetHttpHandler(requestContext);
        }

        private void Decorate(RequestContext requestContext)
        {
            foreach (var decorator in decorators)
            {
                decorator.Decorate(requestContext);
            }
        }
    }
}
