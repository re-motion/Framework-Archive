using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Remotion.Configuration;

namespace Remotion.Core.UnitTests.Configuration
{
  public abstract class FakeProviderBase : ExtendedProviderBase
  {
    protected FakeProviderBase (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}