using System;
using System.Collections.Specialized;
using System.Configuration.Provider;

namespace Rubicon.Configuration
{
  public abstract class ExtendedProviderBase: ProviderBase
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    protected ExtendedProviderBase (string name, NameValueCollection config)
    {
      Initialize (name, config);
    }

    [Obsolete ("TODO: Remove after test")]
    protected ExtendedProviderBase()
    {
    }

    // methods and properties

    public override sealed void Initialize (string name, NameValueCollection config)
    {
      base.Initialize (name, config);
    }
  }
}