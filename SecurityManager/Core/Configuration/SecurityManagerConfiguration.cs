using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using Rubicon.Configuration;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

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
              s_current = (SecurityManagerConfiguration) ConfigurationManager.GetSection ("rubicon.securityManager.domain");
              if (s_current == null)
                s_current = new SecurityManagerConfiguration ();
            }
          }
        }

        return s_current;
      }
    }

    private IOrganizationalStructureFactory _organizationalStructureFactory;

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _xmlnsProperty;
    private readonly ConfigurationProperty _customOrganizationalStructureFactoryProperty;

    public SecurityManagerConfiguration ()
    {
      _properties = new ConfigurationPropertyCollection ();
      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      _customOrganizationalStructureFactoryProperty = new ConfigurationProperty ("customOrganizationalStructureFactory",
          typeof (TypeElement<IOrganizationalStructureFactory>), null, ConfigurationPropertyOptions.None);

      _properties.Add (_xmlnsProperty);
      _properties.Add (_customOrganizationalStructureFactoryProperty);
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
          _organizationalStructureFactory = GetOrganizationalStructureFactory ();

        return _organizationalStructureFactory;
      }
      set
      {
        _organizationalStructureFactory = value;
      }
    }

    private IOrganizationalStructureFactory GetOrganizationalStructureFactory ()
    {
      TypeElement<IOrganizationalStructureFactory> customOrganizationalStructureFactoryElement =
          (TypeElement<IOrganizationalStructureFactory>) this[_customOrganizationalStructureFactoryProperty];

      if (customOrganizationalStructureFactoryElement.Type == null)
        return new OrganizationalStructureFactory ();

      return (IOrganizationalStructureFactory) Activator.CreateInstance (customOrganizationalStructureFactoryElement.Type);
    }
  }
}
