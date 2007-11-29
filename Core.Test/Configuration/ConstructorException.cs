using System;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class ConstructorException : Exception
  {
    public ConstructorException (string message)
        : base (message)
    {
    }
  }
}