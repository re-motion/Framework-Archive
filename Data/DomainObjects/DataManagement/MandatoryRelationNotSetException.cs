using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.DataManagement
{
[Serializable]
public class MandatoryRelationNotSetException : DataManagementException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public MandatoryRelationNotSetException () : this ("A mandatory relation was not set.") {}
  public MandatoryRelationNotSetException (string message) : base (message) {}
  public MandatoryRelationNotSetException (string message, Exception inner) : base (message, inner) {}
  protected MandatoryRelationNotSetException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
