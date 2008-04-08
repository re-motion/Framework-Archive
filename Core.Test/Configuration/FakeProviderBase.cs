using System;
using System.Collections.Specialized;
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