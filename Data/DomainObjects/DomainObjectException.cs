using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects
{
/// <summary>
/// BaseClass for all exceptions of the persistence framework.
/// </summary>
[Serializable]
public class DomainObjectException : Exception
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DomainObjectException () : this ("A domain object exception occurred.") {}
  public DomainObjectException (string message) : base (message) {}
  public DomainObjectException (string message, Exception inner) : base (message, inner) {}
  protected DomainObjectException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
