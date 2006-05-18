using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class SecurityConfiguration : ConfigurationSection
  {
    // types

    // static members

    private static SecurityConfiguration s_current = null;

    public static SecurityConfiguration Current
    {
      get
      {
        if (s_current == null)
        {
          lock (typeof (SecurityConfiguration))
          {
            if (s_current == null)
            {
              s_current = new SecurityConfiguration ();
            }
          }
        }

        return s_current;
      }
    }

    // member fields

    private ISecurityService _securityService;
    private IUserProvider _userProvider;

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _customSecurityServiceProperty;
    private readonly ConfigurationProperty _securityServiceTypeProperty;
    private readonly ConfigurationProperty _customUserProviderProperty;
    private readonly ConfigurationProperty _userProviderTypeProperty;

    // construction and disposing

    public SecurityConfiguration ()
    {
      _customSecurityServiceProperty =
          new ConfigurationProperty ("customService", typeof (SecurityServiceElement), null, ConfigurationPropertyOptions.None);
      _securityServiceTypeProperty =
          new ConfigurationProperty ("service", typeof (SecurityServiceType), SecurityServiceType.None, ConfigurationPropertyOptions.None);
      _customUserProviderProperty =
          new ConfigurationProperty ("customUserProvider", typeof (UserProviderElement), null, ConfigurationPropertyOptions.None);
      _userProviderTypeProperty =
          new ConfigurationProperty ("userProvider", typeof (UserProviderType), UserProviderType.Thread, ConfigurationPropertyOptions.None);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_customSecurityServiceProperty);
      _properties.Add (_securityServiceTypeProperty);
      _properties.Add (_customUserProviderProperty);
      _properties.Add (_userProviderTypeProperty);
    }

    // methods and properties

    public ISecurityService SecurityService
    {
      get
      {
        if (_securityService == null)
        {
          if (SecurityServiceType == SecurityServiceType.Custom)
            _securityService = (ISecurityService) Activator.CreateInstance (CustomService.Type);
        }
        return _securityService;
      }
      set
      {
        _securityService = value;
      }
    }

    public IUserProvider UserProvider
    {
      get
      {
        if (_userProvider == null)
        {
          switch (UserProviderType)
          {
            case UserProviderType.Thread:
              _userProvider = new ThreadUserProvider ();
              break;
            case UserProviderType.Custom:
              _userProvider = (IUserProvider) Activator.CreateInstance (CustomUserProvider.Type);
              break;
          }
        }
        return _userProvider;
      }
      set { _userProvider = value; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    protected SecurityServiceElement CustomService
    {
      get { return (SecurityServiceElement) this[_customSecurityServiceProperty]; }
      set { this[_customSecurityServiceProperty] = value; }
    }

    protected SecurityServiceType SecurityServiceType
    {
      get { return (SecurityServiceType) this[_securityServiceTypeProperty]; }
      set { this[_securityServiceTypeProperty] = value; }
    }

    protected UserProviderElement CustomUserProvider
    {
      get { return (UserProviderElement) this[_customUserProviderProperty]; }
      set { this[_customUserProviderProperty] = value; }
    }

    protected UserProviderType UserProviderType
    {
      get { return (UserProviderType) this[_userProviderTypeProperty]; }
      set { this[_userProviderTypeProperty] = value; }
    }
  }

  public enum SecurityServiceType
  {
    None,
    SecurityManagerService,
    SecurityManagerWebService,
    Custom
  }

  public enum UserProviderType
  {
    Thread,
    HttpContext,
    Custom
  }
}
