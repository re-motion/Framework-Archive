using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects
{
//Documentation: All done

/// <summary>
/// BaseClass for exceptions that are related to the configuraton of the persistence framework.
/// </summary>
public class ConfigurationException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConfigurationException () {}
  public ConfigurationException (string message) : base (message) {}
  public ConfigurationException (string message, Exception inner) : base (message, inner) {}
  protected ConfigurationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
