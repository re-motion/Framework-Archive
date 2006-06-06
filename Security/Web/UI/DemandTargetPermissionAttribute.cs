using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UI
{
  public enum PermissionSource
  {
    SecurableObject,
    WxeFunction
  }

  public abstract class DemandTargetPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private PermissionSource _permissionSource;
    private Type _functionType;
    private string _methodName;

    // construction and disposing

    protected DemandTargetPermissionAttribute (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("functionType", functionType, typeof (WxeFunction));

      _permissionSource = PermissionSource.WxeFunction;
      _functionType = functionType;
    }

    protected DemandTargetPermissionAttribute (string methodName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      _permissionSource = PermissionSource.SecurableObject;
      _methodName = methodName;
    }

    // methods and properties

    public PermissionSource PermissionSource
    {
      get { return _permissionSource; }
    }
	
    public Type FunctionType
    {
      get { return _functionType; }
    }

    public string MethodName
    {
      get { return _methodName; }
    }
  }
}