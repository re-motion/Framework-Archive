using System;
using System.Runtime.Serialization;

namespace Rubicon.Data.DomainObjects.Persistence
{
[Serializable]
public class RdbmsExpressionSecurityException : PersistenceException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RdbmsExpressionSecurityException () : this ("Invalid character in an RDBMS expression encountered.") {}
  public RdbmsExpressionSecurityException (string message) : base (message) {}
  public RdbmsExpressionSecurityException (string message, Exception inner) : base (message, inner) {}
  protected RdbmsExpressionSecurityException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
