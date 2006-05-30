using System;
using System.Runtime.Serialization;

namespace Rubicon.Security
{
  [Serializable]
  public class PermissionDeniedException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PermissionDeniedException () : this ("The operation is not allowed.") {}
    public PermissionDeniedException (string message) : base (message) {}
    public PermissionDeniedException (string message, Exception inner) : base (message, inner) {}
    protected PermissionDeniedException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
