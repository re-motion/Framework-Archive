using System;
using System.Collections;
using System.Web.UI;

using Rubicon.Globalization;

namespace Rubicon.Web.UI.Globalization
{

/// <summary>
///   Interface for controls who wish to use automatic resource dispatching
///   but implement the dispatching logic themselves.
/// </summary>
public interface IResourceDispatchTarget
{
  /// <summary>
  ///   <c>Dispatch</c> is called by the parent control
  ///   and receives the resources as an <c>IDictonary</c>.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     The implementation of <c>IResourceDispatchTarget</c> is responsible for interpreting
  ///     the resources provided through <c>Dispatch</c>.
  ///   </para><para>
  ///     The key of the <c>IDictonaryEntry</c> can be a simple property name
  ///     or a more complex string. It can be freely defined by the <c>IResourceDispatchTarget</c>
  ///     implementation. Inside the resource container, this key is prepended by the control
  ///     instance's ID and a prefix.For details, please refer to 
  ///     <see cref="ResourceDispatcher.Dispatch(Control, IResourceManager)" />
  ///   </para>
  /// </remarks>
  /// <param name="values">
  ///   An <c>IDictonary</c> consisting of name/value pairs of strings.
  /// </param>
  void Dispatch (IDictionary values);
}

}