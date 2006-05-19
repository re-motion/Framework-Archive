using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Reflection;

using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class SecurityConfiguration : ConfigurationSection
  {
    // types

    // static members

    private static readonly string s_httpContextUserProviderName;
    private static SecurityConfiguration s_current;

    static SecurityConfiguration ()
    {
      AssemblyName assemblyName = typeof (SecurityConfiguration).Assembly.GetName ();
      string assemblyNameSuffix = assemblyName.FullName.Substring (assemblyName.Name.Length);
      s_httpContextUserProviderName = "Rubicon.Security.Web.HttpContextUserProvider, Rubicon.Security.Web" + assemblyNameSuffix;
    }

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
          switch (SecurityServiceType)
          {
            case SecurityServiceType.None:
              _securityService = null;
              break;
            case SecurityServiceType.Custom:
              _securityService = (ISecurityService) Activator.CreateInstance (CustomService.Type);
              break;
            default:
              throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the SecurityService.", SecurityServiceType));
          }
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
            case UserProviderType.None:
              _userProvider = null;
              break;
            case UserProviderType.Thread:
              _userProvider = new ThreadUserProvider ();
              break;
            case UserProviderType.HttpContext:
              Type type = Type.GetType (s_httpContextUserProviderName, false, false);
              if (type == null)
              {
                throw new TypeLoadException (string.Format ("Cannot load type '{0}' required by configuration setting userProvider=HttpContext. "
                        + "If the configuration belongs to a web application, please ensure that the assembly 'Rubicon.Web.Security' is located "
                        + "within the CLR's probing path for this application. If the configuration belongs to a any other kind of application "
                        + "(e.g. a Windows Forms or a Console application), you cannot use the HttpContext as a user provider.",
                    s_httpContextUserProviderName));            
              }
              _userProvider = (IUserProvider) Activator.CreateInstance (type);
              break;
            case UserProviderType.Custom:
              _userProvider = (IUserProvider) Activator.CreateInstance (CustomUserProvider.Type);
              break;
            default:
              throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the UserProvider.", UserProviderType));
          }
        }
        return _userProvider;
      }
      set 
      {
        _userProvider = value; 
      }
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
    //SecurityManagerService,
    //SecurityManagerWebService,
    Custom
  }

  public enum UserProviderType
  {
    None,
    Thread,
    HttpContext,
    Custom
  }
}
