using System;

namespace Rubicon.Data.DomainObjects.Transport
{
  public class TransportationException : Exception
  {
    public TransportationException ()
    {
    }

    public TransportationException (string message)
        : base (message)
    {
    }

    public TransportationException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

  }
}