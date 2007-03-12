using System;
using System.Collections.Specialized;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class FakeProvider : FakeProviderBase, IFakeProvider
  {
    public FakeProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}