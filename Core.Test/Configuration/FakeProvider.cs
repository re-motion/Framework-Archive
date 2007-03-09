using System;
using System.Collections.Specialized;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class FakeProvider: ExtendedProviderBase, IFakeProvider
  {
    public FakeProvider()
    {
    }

    public FakeProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}