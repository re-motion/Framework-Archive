using System;
using System.Globalization;
using System.Resources;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Reflection;
using System.Runtime.Serialization;

namespace Rubicon.Globalization
{

[Serializable]
public class ResourceException: Exception
{
  public ResourceException (string message)
    : base (message)
  {
  }

  public ResourceException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  public ResourceException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}