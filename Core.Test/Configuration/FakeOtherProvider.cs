using System;
using System.Collections.Specialized;
using Remotion.Configuration;

namespace Remotion.Core.UnitTests.Configuration
{
  public class FakeOtherProvider : ExtendedProviderBase
  {
    public FakeOtherProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}