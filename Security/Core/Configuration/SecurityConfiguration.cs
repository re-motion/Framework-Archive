using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Reflection;

using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.Configuration
{
  public class SecurityConfiguration : ConfigurationSection
  {
    // types

    // static members

    private static SecurityConfiguration s_current;

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
              s_current = (SecurityConfiguration) ConfigurationManager.GetSection ("rubicon.security");
              if (s_current == null)
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
    private IFunctionalSecurityStrategy _functionalSecurityStrategy;
    private IPermissionProvider _permissionProvider;

    private ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _xmlnsProperty;
    private readonly ConfigurationProperty _customSecurityServiceProperty;
    private readonly ConfigurationProperty _securityServiceTypeProperty;
    private readonly ConfigurationProperty _customUserProviderProperty;
    private readonly ConfigurationProperty _userProviderTypeProperty;
    private readonly ConfigurationProperty _customPermissionProviderProperty;
    private Type _httpContextUserProviderType;

    // construction and disposing

    public SecurityConfiguration ()
    {
      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      _customSecurityServiceProperty =
          new ConfigurationProperty ("customService", typeof (TypeElement<ISecurityService>), null, ConfigurationPropertyOptions.None);
      _securityServiceTypeProperty =
          new ConfigurationProperty ("service", typeof (SecurityServiceType), SecurityServiceType.None, ConfigurationPropertyOptions.None);
      _customUserProviderProperty =
          new ConfigurationProperty ("customUserProvider", typeof (TypeElement<IUserProvider>), null, ConfigurationPropertyOptions.None);
      _userProviderTypeProperty =
          new ConfigurationProperty ("userProvider", typeof (UserProviderType), UserProviderType.Thread, ConfigurationPropertyOptions.None);
      _customPermissionProviderProperty =
          new ConfigurationProperty ("customPermissionProvider", typeof (TypeElement<IPermissionProvider>), null, ConfigurationPropertyOptions.None);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_xmlnsProperty);
      _properties.Add (_customSecurityServiceProperty);
      _properties.Add (_securityServiceTypeProperty);
      _properties.Add (_customUserProviderProperty);
      _properties.Add (_userProviderTypeProperty);
      _properties.Add (_customPermissionProviderProperty);
    }

    // methods and properties

    public ISecurityService SecurityService
    {
      get
      {
        if (_securityService == null)
          _securityService = GetSecurityServiceFromConfiguration ();
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
          _userProvider = GetUserProviderFromConfiguration ();
        return _userProvider;
      }
      set
      {
        _userProvider = value;
      }
    }

    public IPermissionProvider PermissionProvider
    {
      get
      {
        if (_permissionProvider == null)
          _permissionProvider = GetPermissionProviderFromConfiguration ();

        return _permissionProvider;
      }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get
      {
        if (_functionalSecurityStrategy == null)
          _functionalSecurityStrategy = new FunctionalSecurityStrategy ();

        return _functionalSecurityStrategy;
      }
      set
      {
        _functionalSecurityStrategy = value;
      }
    }

    private ISecurityService GetSecurityServiceFromConfiguration ()
    {
      switch (SecurityServiceType)
      {
        case SecurityServiceType.None:
          return null;
        case SecurityServiceType.Custom:
          return (ISecurityService) Activator.CreateInstance (CustomService.Type);
        default:
          throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the SecurityService.", SecurityServiceType));
      }
    }

    private IUserProvider GetUserProviderFromConfiguration ()
    {
      switch (UserProviderType)
      {
        case UserProviderType.None:
          return null;
        case UserProviderType.Thread:
          return new ThreadUserProvider ();
        case UserProviderType.HttpContext:
          if (_httpContextUserProviderType == null)
            _httpContextUserProviderType = GetHttpContextUserProviderType ();
          return (IUserProvider) Activator.CreateInstance (_httpContextUserProviderType);
        case UserProviderType.Custom:
          return (IUserProvider) Activator.CreateInstance (CustomUserProvider.Type);
        default:
          throw new InvalidOperationException (string.Format ("Choice '{0}' is not supported when selecting the UserProvider.", UserProviderType));
      }
    }

    private IPermissionProvider GetPermissionProviderFromConfiguration ()
    {
      if (CustomPermissionProvider.Type == null)
        return new PermissionReflector ();

      return (IPermissionProvider) Activator.CreateInstance (CustomPermissionProvider.Type);
    }

    private Type GetHttpContextUserProviderType ()
    {
      AssemblyName securityAssemblyName = typeof (SecurityConfiguration).Assembly.GetName ();
      AssemblyName securityWebAssemblyName = new AssemblyName (securityAssemblyName.FullName);
      securityWebAssemblyName.Name = "Rubicon.Security.Web";

      try
      {
        Assembly.Load (securityWebAssemblyName);
      }
      catch (FileNotFoundException e)
      {
        throw new ConfigurationErrorsException (string.Format ("The current value of property 'userProvider' requires that the assembly "
                + "'Rubicon.Security.Web' must be placed within the CLR's probing path for this application. The error is: {0}", e.Message),
            e,
            ElementInformation.Properties[_userProviderTypeProperty.Name].Source,
            ElementInformation.Properties[_userProviderTypeProperty.Name].LineNumber);
      }

      return Type.GetType (Assembly.CreateQualifiedName (securityWebAssemblyName.FullName, "Rubicon.Security.Web.HttpContextUserProvider"), true, false);
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize ();

      if (UserProviderType == UserProviderType.HttpContext)
        _httpContextUserProviderType = GetHttpContextUserProviderType ();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    protected TypeElement<ISecurityService> CustomService
    {
      get { return (TypeElement<ISecurityService>) this[_customSecurityServiceProperty]; }
      set { this[_customSecurityServiceProperty] = value; }
    }

    protected SecurityServiceType SecurityServiceType
    {
      get { return (SecurityServiceType) this[_securityServiceTypeProperty]; }
      set { this[_securityServiceTypeProperty] = value; }
    }

    protected TypeElement<IUserProvider> CustomUserProvider
    {
      get { return (TypeElement<IUserProvider>) this[_customUserProviderProperty]; }
      set { this[_customUserProviderProperty] = value; }
    }

    protected UserProviderType UserProviderType
    {
      get { return (UserProviderType) this[_userProviderTypeProperty]; }
      set { this[_userProviderTypeProperty] = value; }
    }

    protected TypeElement<IPermissionProvider> CustomPermissionProvider
    {
      get { return (TypeElement<IPermissionProvider>) this[_customPermissionProviderProperty]; }
      set { this[_customPermissionProviderProperty] = value; }
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
