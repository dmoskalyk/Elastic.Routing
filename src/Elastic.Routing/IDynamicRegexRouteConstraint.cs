using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elastic.Routing
{
    /// <summary>
    /// An interface representing a route constraint with regular expression pattern which can be changed on runtime.
    /// </summary>
    public interface IDynamicRegexRouteConstraint : IRegexRouteConstraint
    {
        /// <summary>
        /// An evenet which occurs when <see cref="IRegexRouteConstraint.Pattern"/> is changed.
        /// </summary>
        event EventHandler RegexChanged;
    }
}
