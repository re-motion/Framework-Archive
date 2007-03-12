using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public abstract class FakeProviderBase : ExtendedProviderBase
  {
    protected FakeProviderBase (string name, NameValueCollection config)
        : base (name, config)
    {
    }
  }
}