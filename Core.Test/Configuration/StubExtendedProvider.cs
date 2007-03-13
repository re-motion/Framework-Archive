using System;
using System.Collections.Specialized;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class StubExtendedProvider: ExtendedProviderBase
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing


    public StubExtendedProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public new string GetAndRemoveNonEmptyStringAttribute (NameValueCollection config, string attribute, string providerName, bool required)
    {
     return base.GetAndRemoveNonEmptyStringAttribute (config, attribute, providerName, required);
    }
  }
}