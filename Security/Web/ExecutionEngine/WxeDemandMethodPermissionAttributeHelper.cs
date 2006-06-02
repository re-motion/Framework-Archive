using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public class WxeDemandMethodPermissionAttributeHelper
  {
    // types

    // static members

    // member fields

    private Type _functionType;
    private WxeDemandMethodPermissionAttribute _attribute;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelper (Type functionType, WxeDemandMethodPermissionAttribute attribute)
    {
      ArgumentUtility.CheckNotNull ("functionType", functionType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      switch (attribute.Type)
      {
        case MethodType.Instance:
          CheckMethodNameNotNullOrEmpty (functionType, attribute.MethodName);
          break;
        case MethodType.Static:
          CheckSecurabeClassNotNull (functionType, attribute.SecurableClass);
          CheckMethodNameNotNullOrEmpty (functionType, attribute.MethodName);
          break;
        case MethodType.Constructor:
          CheckSecurabeClassNotNull (functionType, attribute.SecurableClass);
          break;
      }

      _functionType = functionType;
      _attribute = attribute;
    }

    // methods and properties

    public Type FunctionType
    {
      get { return _functionType; }
    }

    public MethodType MethodType
    {
      get { return _attribute.Type; }
    }

    public string MethodName
    {
      get { return _attribute.MethodName; }
    }

    public Type SecurableClass
    {
      get { return _attribute.SecurableClass; }
    }

    public Type GetTypeOfSecurableObject ()
    {
      WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParameterDeclarations (_functionType);
      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (parameterDeclarations);
      if (!typeof (ISecurableObject).IsAssignableFrom (parameterDeclaration.Type))
      {
        throw new WxeException (string.Format ("The parameter '{0}' specified by the WxeDemandMethodPermissionAttribute applied to WxeFunction '{1}'"
                + " does not implement interface '{2}'.",
            parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      return parameterDeclaration.Type;
    }

    public ISecurableObject GetSecurableObject (WxeFunction function)
    {
      ArgumentUtility.CheckNotNullAndType ("function", function, _functionType);

      WxeParameterDeclaration parameterDeclaration = GetParameterDeclaration (function.ParameterDeclarations);
      ISecurableObject securableObject = function.Variables[parameterDeclaration.Name] as ISecurableObject;
      if (securableObject == null)
      {
        throw new WxeException (string.Format ("The parameter '{0}' specified by the WxeDemandMethodPermissionAttribute applied to WxeFunction '{1}'"
                + " is null or does not implement interface '{2}'.",
            parameterDeclaration.Name, _functionType.FullName, typeof (ISecurableObject).FullName));
      }

      return securableObject;
    }

    private WxeParameterDeclaration GetParameterDeclaration (WxeParameterDeclaration[] parameterDeclarations)
    {
      if (parameterDeclarations.Length == 0)
      {
        throw new WxeException (string.Format (
            "WxeFunction '{0}' has a WxeDemandMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.",
            _functionType.FullName));
      }

      if (StringUtility.IsNullOrEmpty (_attribute.ParameterName))
      {
        return parameterDeclarations[0];
      }
      else
      {
        for (int i = 0; i < parameterDeclarations.Length; i++)
        {
          if (string.Equals (parameterDeclarations[i].Name, _attribute.ParameterName, StringComparison.Ordinal))
            return parameterDeclarations[i];
        }
      }

      throw new WxeException (string.Format ("The parameter '{0}' specified by the WxeDemandMethodPermissionAttribute applied to WxeFunction '{1}'"
              + " is not a valid parameter of this function.",
          _attribute.ParameterName, _functionType.FullName));
    }

    private void CheckMethodNameNotNullOrEmpty (Type functionType, string methodName)
    {
      if (StringUtility.IsNullOrEmpty (methodName))
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify the method to get the required permissions from.",
            functionType.FullName));
      }
    }

    private void CheckSecurabeClassNotNull (Type functionType, Type securableClass)
    {
      if (securableClass == null)
      {
        throw new WxeException (string.Format (
            "The WxeDemandMethodPermissionAttribute applied to WxeFunction '{0}' does not specify a type implementing interface '{1}'.",
            functionType.FullName, typeof (ISecurableObject).FullName));
      }
    }
  }
}