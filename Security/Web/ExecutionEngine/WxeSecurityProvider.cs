using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public class WxeSecurityProvider
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

      WxeDemandMethodPermissionAttribute attribute =
          (WxeDemandMethodPermissionAttribute) Attribute.GetCustomAttribute (function.GetType (), typeof (WxeDemandMethodPermissionAttribute), true);

      if (attribute == null)
        return;

      switch (attribute.Type)
      {
        case MethodType.Instance:
          CheckMethodAccess (function, attribute.ParameterName, attribute.Method);
          break;
        case MethodType.Static:
          CheckStaticMethodAccess (function, attribute.SecurableClass, attribute.Method);
          break;
        case MethodType.Constructor:
          CheckConstructorAccess (attribute.SecurableClass);
          break;
        default:
          throw new InvalidOperationException (string.Format ("MethodType '{0}' is not supported.", attribute.Type));
      }
    }

    private void CheckMethodAccess (WxeFunction function, string parameterName, string method)
    {
      if (StringUtility.IsNullOrEmpty (parameterName))
      {
        if (function.ParameterDeclarations.Length == 0)
        {
          throw new WxeException (string.Format (
              "WxeFunction '{0}' has a WxeDemandMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.",
              GetType ().Name));
        }
        parameterName = function.ParameterDeclarations[0].Name;
      }

      if (StringUtility.IsNullOrEmpty (method))
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a method to get the requied permissions from.",
            GetType ().Name));
      }

      ISecurableObject securableObject = function.Variables[parameterName] as ISecurableObject;
      if (securableObject == null)
        throw new WxeException (string.Format ("Parameter '{0}' is null or does not implement interface 'Rubicon.Security.ISecurableObject'.", parameterName));

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckMethodAccess (securableObject, method);
    }

    private void CheckStaticMethodAccess (WxeFunction function, Type securableClass, string method)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.",
            GetType ().Name));
      }

      if (StringUtility.IsNullOrEmpty (method))
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a method to get the requied permissions from.",
            GetType ().Name));
      }

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckStaticMethodAccess (securableClass, method);
    }

    private void CheckConstructorAccess (Type securableClass)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a type implementing interface 'Rubicon.Security.ISecurableObject'.",
            GetType ().Name));
      }

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      securityClient.CheckConstructorAccess (securableClass);
    }
  }
}