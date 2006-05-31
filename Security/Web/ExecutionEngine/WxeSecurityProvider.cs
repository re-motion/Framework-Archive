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

    public bool HasAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      WxeDemandMethodPermissionAttribute attribute = GetPermissionAttribute (functionType);
      if (attribute == null)
        return true;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          return HasInstanceMethodAccess (functionType, attribute.ParameterName, attribute.Method);
        //case MethodType.Static:
        //  HasStaticMethodAccess (functionType, attribute.SecurableClass, attribute.Method);
        //case MethodType.Constructor:
        //  HasConstructorAccess (functionType, attribute.SecurableClass);
        default:
          throw new InvalidOperationException (string.Format ("MethodType '{0}' is not supported.", attribute.Type));
      }
    }

    private bool HasInstanceMethodAccess (Type functionType, string parameterName, string method)
    {
      WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParameterDeclarations (functionType);
      
      if (StringUtility.IsNullOrEmpty (parameterName))
        parameterName = GetParameterNameOfSecurableObject (functionType, parameterDeclarations, parameterName);
      CheckMethodNotNullOrEmpty (functionType, method);

      Type securableClass = GetTypeOfSecurableObject (functionType, parameterDeclarations, parameterName);

      Enum[] requiredAccessTypeEnums = SecurityConfiguration.Current.PermissionProvider.GetRequiredMethodPermissions (securableClass, method);

      return SecurityConfiguration.Current.FunctionalSecurityStrategy.HasAccess (
          securableClass, 
          SecurityConfiguration.Current.SecurityService, 
          SecurityConfiguration.Current.UserProvider.GetUser(),
          Array.ConvertAll (requiredAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get)));
    }

    public void CheckAccess (WxeFunction function)
    {
      ArgumentUtility.CheckNotNull ("function", function);

      WxeDemandMethodPermissionAttribute attribute = GetPermissionAttribute (function.GetType ());
      if (attribute == null)
        return;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          CheckInstanceMethodAccess (function, attribute.ParameterName, attribute.Method);
          break;
        case MethodType.Static:
          CheckStaticMethodAccess (function, attribute.SecurableClass, attribute.Method);
          break;
        case MethodType.Constructor:
          CheckConstructorAccess (function, attribute.SecurableClass);
          break;
        default:
          throw new InvalidOperationException (string.Format ("MethodType '{0}' is not supported.", attribute.Type));
      }
    }

    private void CheckInstanceMethodAccess (WxeFunction function, string parameterName, string method)
    {
      if (StringUtility.IsNullOrEmpty (parameterName))
        parameterName = GetParameterNameOfSecurableObject (function.GetType (), function.ParameterDeclarations, parameterName);
      CheckMethodNotNullOrEmpty (function.GetType (), method);

      ISecurableObject securableObject = function.Variables[parameterName] as ISecurableObject;
      if (securableObject == null)
        throw new WxeException (string.Format ("The Parameter '{0}' is null or does not implement interface 'Rubicon.Security.ISecurableObject'.", parameterName));

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckMethodAccess (securableObject, method);
    }

    private void CheckStaticMethodAccess (WxeFunction function, Type securableClass, string method)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.",
            function.GetType ().Name));
      }

      CheckMethodNotNullOrEmpty (function.GetType (), method);

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckStaticMethodAccess (securableClass, method);
    }

    private void CheckConstructorAccess (WxeFunction function, Type securableClass)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.",
            function.GetType ().Name));
      }

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckConstructorAccess (securableClass);
    }

    private WxeDemandMethodPermissionAttribute GetPermissionAttribute (Type functionType)
    {
      return (WxeDemandMethodPermissionAttribute) Attribute.GetCustomAttribute (functionType, typeof (WxeDemandMethodPermissionAttribute), true);
    }

    private string GetParameterNameOfSecurableObject (Type functionType, WxeParameterDeclaration[] parameterDeclarations, string parameterName)
    {
      if (parameterDeclarations.Length == 0)
      {
        throw new WxeException (string.Format (
            "WxeFunction '{0}' has a WxeDemandMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.",
            functionType.Name));
      }
      return parameterDeclarations[0].Name;
    }

    private static void CheckMethodNotNullOrEmpty (Type functionType, string method)
    {
      if (StringUtility.IsNullOrEmpty (method))
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a method to get the required permissions from.",
            functionType.Name));
      }
    }

    private Type GetTypeOfSecurableObject (Type functionType, WxeParameterDeclaration[] parameterDeclarations, string parameterName)
    {
      WxeParameterDeclaration parameterDeclaration = null;
      for (int i = 0; i < parameterDeclarations.Length; i++)
      {
        if (string.Equals (parameterDeclarations[i].Name, parameterName, StringComparison.Ordinal))
        {
          parameterDeclaration = parameterDeclarations[i];
          break;
        }
      }

      if (parameterDeclaration == null)
      {
        throw new WxeException (string.Format ("The parameter '{0}' specified by the WxeDemandMethodPermissionAttribute applied to WxeFunction '{1}'"
                + " is not a valid parameter of this function.",
            parameterName, functionType));
      }

      if (!typeof (ISecurableObject).IsAssignableFrom (parameterDeclaration.Type))
      {
        throw new WxeException (string.Format ("The type of parameter '{0}' of WxeFunction '{1}' does not implement interface"
                + " 'Rubicon.Security.ISecurableObject'.",
            parameterName, functionType.Name));
      }

      return parameterDeclaration.Type;
    }
  }
}
