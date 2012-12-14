using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Persistence
{
[Serializable]
public class ConcurrencyViolationException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ConcurrencyViolationException () : this ("Concurrency violation encountered.") {}
  public ConcurrencyViolationException (string message) : base (message) {}
  public ConcurrencyViolationException (string message, Exception inner) : base (message, inner) {}
  protected ConcurrencyViolationException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
