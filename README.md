# Elastic.Routing

Flexible routing extensions for ASP.NET 4.
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

Wildcard parameter names starts with `*`.

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

**2. Optional segments**

Optional URL are surrounded by braces.

**Be very careful with optional parameters because in some cases the URL can be parsed in several different ways.**

Example:

    ({lang}/){controller}/{action}/({id})

This pattern will match the following urls:

* en/records/list/
  * _lang_: en
  * _controller_: records
  * _action_: list
  * _id_: <empty>
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

**3. Constraints**

Constraints work in the same way as in the standard ASP.NET Routing except that they are not evaluated for optional parameters which have not been matched in the incoming URL.

Constraints can be strings of regular expression format or objects of type `IRouteConstraint` which comes from the standard `System.Web.Routing` library.

**Constraints are not evaluated on default values (both incoming and outgoing).**

**4. Default values**

The route uses two separate collections of default values:

* Incoming - used when parsing a URL and the parameter is optional.
* Outgoing - used for URL construction, for example, to provide some value from the application/session/request state.

A default value can be a simple object whose `ToString()` method is evaluated or an instance which implements the `IRouteValueProvider` interface.

Elastic.Routing is copyrighted under [the following license](https://github.com/lokiworld/Elastic.Routing/blob/master/LICENSE.txt).