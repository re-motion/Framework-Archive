using System;
using System.ComponentModel;

namespace Rubicon.ObjectBinding.Design
{

/// <summary>
///   This interface allows components to determine the expression used to bind to them.
/// </summary>
/// <remarks>
///   By default, Site.Name is used to create a binding expression like Datasource=&lt;% ComponentIdentifier %&gt;.
///   However, this does not work for components that do not expose their site name at compile time, like pages
///   and user controls. These items can implement this interface to return a different binding.
///   Note that the template control containing must be able to resolve these expressions using <see cref="IResolveComponentBindingExpression"/>.
/// </remarks>
public interface IGetComponentBindingExpression
{
  string BindingExpression { get; }
}

/// <summary>
///   This interface allows template controls to resolve special expression created by <see cref="IGetComponentBindingExpression"/>.
/// </summary>
public interface IResolveComponentBindingExpression
{
  IComponent Resolve (string expression);
}

}