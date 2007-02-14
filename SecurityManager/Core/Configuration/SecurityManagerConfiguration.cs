using System;
using System.Configuration;
using Rubicon.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Configuration
{
  public class SecurityManagerConfiguration : ConfigurationSection
  {
    private static SecurityManagerConfiguration s_current;

    public static SecurityManagerConfiguration Current
    {
      get
      {
        if (s_current == null)
        {
          lock (typeof (SecurityManagerConfiguration))
          {
            if (s_current == null)
            {
              s_current = (SecurityManagerConfiguration) ConfigurationManager.GetSection ("rubicon.securityManager");
              if (s_current == null)
                s_current = new SecurityManagerConfiguration();
            }
          }
        }

        return s_current;
      }
    }

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _xmlnsProperty;
    
    private readonly object _lockOrganizationalStructureFactory = new object();
    private readonly ConfigurationProperty _organizationalStructureFactoryProperty;
    private IOrganizationalStructureFactory _organizationalStructureFactory;

    public SecurityManagerConfiguration()
    {
      _properties = new ConfigurationPropertyCollection();
      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      _organizationalStructureFactoryProperty = new ConfigurationProperty (
          "organizationalStructureFactory",
          typeof (TypeElement<IOrganizationalStructureFactory, OrganizationalStructureFactory>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_xmlnsProperty);
      _properties.Add (_organizationalStructureFactoryProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IOrganizationalStructureFactory OrganizationalStructureFactory
    {
      get
      {
        if (_organizationalStructureFactory == null)
        {
          lock (_lockOrganizationalStructureFactory)
          {
            if (_organizationalStructureFactory == null)
              _organizationalStructureFactory = OrganizationalStructureFactoryProperty.CreateInstance ();
          }
        }
        return _organizationalStructureFactory;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        lock (_lockOrganizationalStructureFactory)
        {
          _organizationalStructureFactory = value;
        }
      }
    }

    protected TypeElement<IOrganizationalStructureFactory> OrganizationalStructureFactoryProperty
    {
      get { return (TypeElement<IOrganizationalStructureFactory>) this[_organizationalStructureFactoryProperty]; }
    }
  }
}