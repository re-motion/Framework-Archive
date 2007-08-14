using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Infrastructure.DPInterception
{
  public class NonInterceptableTypeException : Exception
  {
    public readonly Type Type;

    public NonInterceptableTypeException (string message, Type type)
      : base (message)
    {
      Type = type;
    }

    public NonInterceptableTypeException (string message, Type type, Exception innerException)
      : base (message, innerException)
    {
      Type = type;
    }
  }
}
