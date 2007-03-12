using System;
using System.Collections.Specialized;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class FakeOtherProvider : ExtendedProviderBase
  {
    public FakeOtherProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}