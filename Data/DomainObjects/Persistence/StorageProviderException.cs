using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Persistence
{
[Serializable]
public class StorageProviderException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public StorageProviderException () {}
  public StorageProviderException (string message) : base (message) {}
  public StorageProviderException (string message, Exception inner) : base (message, inner) {}
  protected StorageProviderException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
