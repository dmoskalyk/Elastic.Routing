# Elastic.Routing

An ASP.NET routing extension package with the support of flexible wildcard parameters, optional URL parts, extended constraints etc.
It does not replace the ASP.NET Routing but extends its possibilities.

## Installation

### Elastic.Routing library
Use NuGet to install Elastic.Routing package into an existing web application:

    PM> Install-Package Elastic.Routing

or [download](https://github.com/lokiworld/Elastic.Routing/zipball/master) a zip package.

### Elastic.Routing.Sample application
This is a sample web application which demonstrates the possibilities of the library.

Use NuGet to install Elastic.Routing.Sample package into an existing web application:

    PM> Install-Package Elastic.Routing.Sample


## Features
**1. Wildcard route parameters**

Wildcard parameter names start with `*`.

Example:

    {area}/{*path}/{id}

This pattern will match the following URLs:

* home/category1/category2/item1
  * _area_: home
  * _path_: category1/category2
  * _id_: item1
* account/details/user1
  * _area_: account
  * _path_: details
  * _id_: user1

<hr/>

**2. Optional segments**

Optional URL segments are surrounded with braces.

**Be very careful with optional parameters because in some cases one URL can be parsed in several different ways.**

Example:

    ({lang}/){controller}/{action}/({id})

This pattern will match the following urls:

* en/records/list/
  * _lang_: en
  * _controller_: records
  * _action_: list
  * _id_: &lt;empty&gt;
* en/records/details/1
  * _lang_: en
  * _controller_: records
  * _action_: details
  * _id_: 1
* records/details/1
  * _lang_: <empty>
  * _controller_: records
  * _action_: details
  * _id_: 1

<hr/>

**3. Constraints**

Constraints work in the same way as in the standard ASP.NET Routing.

Constraint can be a string of regular expression format, an object implementing `IRouteConstraint` interface which comes from the standard `System.Web.Routing` library or an object implementing `IRegexRouteConstraint` which combines both previous options.

**Be aware that constraints are not evaluated on default values (both incoming and outgoing) and optional parameters which have not been matched in the incoming URL.**

<hr/>

**4. Default values**

The route uses two separate collections of default values:

* Incoming - used when parsing a URL and the parameter is optional.
* Outgoing - used for URL construction, for example, to provide some value from the application/session/request state.

A default value can be a simple object whose `ToString()` method is evaluated or an instance which implements the `IRouteValueProvider` interface.

<hr/>

**5. Projections**

Projections transform the route values before the URL is generated of after the route has been parsed. They can be used, for example, to escape the route value, translate it to a different language etc.

Projections are the classes which implement an interface `IRouteValueProjection`.

<hr/>

Elastic.Routing is copyrighted under the [MIT license](https://github.com/lokiworld/Elastic.Routing/blob/master/LICENSE.txt).