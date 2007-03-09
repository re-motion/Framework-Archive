using System;
using System.Configuration;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  public class StubExtendedConfigurationSection: ExtendedConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private StubProviderHelper _stubProviderHelper;

    // construction and disposing

    public StubExtendedConfigurationSection ()
    {
      _stubProviderHelper = new StubProviderHelper (this);
      _stubProviderHelper.InitializeProperties (_properties);
    }

    // methods and properties

    public StubProviderHelper GetStubProviderHelper()
    {
       return _stubProviderHelper;
    }

    public ConfigurationPropertyCollection GetProperties()
    {
      return _properties;
    }

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _stubProviderHelper.PostDeserialze();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }
  }
}