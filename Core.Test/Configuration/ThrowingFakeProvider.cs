using System;
using System.Collections.Specialized;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class ThrowingFakeProvider : FakeProviderBase, IFakeProvider
  {
    public ThrowingFakeProvider (string name, NameValueCollection config)
        : base (name, config)
    {
      throw new ConstructorException ("A message from the constructor.");
    }
  }
}