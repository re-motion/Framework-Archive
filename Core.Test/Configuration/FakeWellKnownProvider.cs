using System;
using System.Collections.Specialized;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class FakeWellKnownProvider: ExtendedProviderBase, IFakeProvider
  {
    public FakeWellKnownProvider()
    {
    }

    public FakeWellKnownProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}