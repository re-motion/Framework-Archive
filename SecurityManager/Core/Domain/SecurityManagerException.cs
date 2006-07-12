using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rubicon.SecurityManager.Domain
{
  [Serializable]
  public class SecurityManagerException : Exception
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurityManagerException () : this ("An error occurred in the security manager.") {}
    public SecurityManagerException (string message) : base (message) {}
    public SecurityManagerException (string message, Exception inner) : base (message, inner) {}
    protected SecurityManagerException (SerializationInfo info, StreamingContext context) : base (info, context) { }

    // methods and properties
  }
}
