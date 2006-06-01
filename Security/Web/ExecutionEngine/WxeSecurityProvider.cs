using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Security.Metadata;
using Rubicon.Security.Configuration;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public class WxeSecurityProvider : IWxeSecurityProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeSecurityProvider ()
    {
    }

    // methods and properties

    public void CheckAccess (WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      WxeDemandMethodPermissionAttribute attribute = GetPermissionAttribute (function.GetType ());
      if (attribute == null)
        return;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          CheckInstanceMethodAccess (function, attribute);
          break;
        case MethodType.Static:
          CheckStaticMethodAccess (function.GetType (), attribute);
          break;
        case MethodType.Constructor:
          CheckConstructorAccess (function.GetType (), attribute);
          break;
        default:
          throw new ArgumentException (string.Format (
              "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' specifies the not supported MethodType '{1}'.",
              function.GetType ().Name, attribute.Type));
      }
    }

    public bool HasAccess (WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      WxeDemandMethodPermissionAttribute attribute = GetPermissionAttribute (function.GetType ());
      if (attribute == null)
        return true;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          return HasInstanceMethodAccess (function, attribute);
        case MethodType.Static:
          return HasStaticMethodAccess (function.GetType (), attribute);
        case MethodType.Constructor:
          return HasConstructorAccess (function.GetType (), attribute);
        default:
          throw new ArgumentException (string.Format (
              "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' specifies the not supported MethodType '{1}'.",
              function.GetType ().Name, attribute.Type));
      }
    }

    public bool HasStatelessAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      WxeDemandMethodPermissionAttribute attribute = GetPermissionAttribute (functionType);
      if (attribute == null)
        return true;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          return HasStatelessInstanceMethodAccess (functionType, attribute);
        case MethodType.Static:
          return HasStaticMethodAccess (functionType, attribute);
        case MethodType.Constructor:
          return HasConstructorAccess (functionType, attribute);
        default:
          throw new ArgumentException (string.Format (
              "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' specifies the not supported MethodType '{1}'.",
              functionType.Name, attribute.Type));
      }
    }

    private WxeDemandMethodPermissionAttribute GetPermissionAttribute (Type functionType)
    {
      return (WxeDemandMethodPermissionAttribute) Attribute.GetCustomAttribute (functionType, typeof (WxeDemandMethodPermissionAttribute), true);
    }

    private bool HasInstanceMethodAccess (WxeFunction function, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (function.GetType (), attribute);
      helper.Validate ();
      
      ISecurableObject securableObject = helper.GetSecurableObject (function);
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasMethodAccess (securableObject, attribute.MethodName);
    }

    private bool HasStatelessInstanceMethodAccess (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      helper.Validate ();

      Type securableClass = helper.GetTypeOfSecurableObject ();
      Enum[] requiredAccessTypeEnums = SecurityConfiguration.Current.PermissionProvider.GetRequiredMethodPermissions (securableClass, attribute.MethodName);

      ISecurityService securityService = SecurityConfiguration.Current.SecurityService;
      if (securityService == null)
        throw new InvalidOperationException (string.Format ("The property 'SecurityService' of the current '{0}' evaluated and returned null.", typeof (SecurityConfiguration).FullName));

      IUserProvider userProvider = SecurityConfiguration.Current.UserProvider;
      if (userProvider == null)
        throw new InvalidOperationException (string.Format ("The property 'UserProvider' of the current '{0}' evaluated and returned null.", typeof (SecurityConfiguration).FullName));

      return SecurityConfiguration.Current.FunctionalSecurityStrategy.HasAccess (
          securableClass,
          securityService,
          userProvider.GetUser (),
          Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get)));
    }

    private bool HasStaticMethodAccess (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      helper.Validate ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasStaticMethodAccess (attribute.SecurableClass, attribute.MethodName);
    }

    private bool HasConstructorAccess (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      helper.Validate ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasConstructorAccess (attribute.SecurableClass);
    }

    private void CheckInstanceMethodAccess (WxeFunction function, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (function.GetType (), attribute);
      helper.Validate ();

      ISecurableObject securableObject = helper.GetSecurableObject (function);
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckMethodAccess (securableObject, attribute.MethodName);
    }

    private void CheckStaticMethodAccess (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      helper.Validate ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckStaticMethodAccess (attribute.SecurableClass, attribute.MethodName);
    }

    private void CheckConstructorAccess (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (functionType, attribute);
      helper.Validate ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckConstructorAccess (attribute.SecurableClass);
    }
  }
}
