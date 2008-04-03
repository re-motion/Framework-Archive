using System;
using System.Collections.Specialized;
using Remotion.Configuration;

namespace Remotion.Core.UnitTests.Configuration
{
  public class FakeWellKnownProvider: ExtendedProviderBase, IFakeProvider
  {
    public FakeWellKnownProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}