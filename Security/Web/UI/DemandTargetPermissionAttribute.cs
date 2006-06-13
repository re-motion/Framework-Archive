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

  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class DemandTargetPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private PermissionSource _permissionSource;
    private Type _functionType;
    private string _methodName;
    private Type _securableClass;

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

    protected DemandTargetPermissionAttribute (string methodName, Type securableClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      _permissionSource = PermissionSource.SecurableObject;
      _methodName = methodName;
      _securableClass = securableClass;
    }

    protected DemandTargetPermissionAttribute (object methodEnum)
    {
      ArgumentUtility.CheckNotNullAndType ("methodEnum", methodEnum, typeof (Enum));

      Enum enumValue = (Enum) methodEnum;
      Type enumType = enumValue.GetType ();

      // TODO: rewrite with test
      if (!typeof (ISecurableObject).IsAssignableFrom (enumType.DeclaringType))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' is not declared as a nested type or the declaring type does not implement interface '{1}'.",
                enumType.FullName,
                typeof (ISecurableObject).FullName),
            "methodEnum");
      }

      _permissionSource = PermissionSource.SecurableObject;
      _securableClass = enumType.DeclaringType;
      _methodName = enumValue.ToString ();
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
  
    public Type SecurableClass
    {
      get { return _securableClass; }
    }
  }
}