using System;
using System.Runtime.Serialization;

namespace Rubicon.Security
{
  [Serializable]
  public class AccessViolationException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AccessViolationException () : this ("The operation is not allowed.") {}
    public AccessViolationException (string message) : base (message) {}
    public AccessViolationException (string message, Exception inner) : base (message, inner) {}
    protected AccessViolationException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
