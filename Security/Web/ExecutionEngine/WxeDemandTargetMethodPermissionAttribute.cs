using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.ComponentModel;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public class WxeDemandTargetMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullAndType ("methodNameEnum", methodNameEnum, typeof (Enum));

      Enum enumValue = (Enum) methodNameEnum;
      Type enumType = enumValue.GetType ();

      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      MethodName = enumValue.ToString ();
      SecurableClass = enumType.DeclaringType;
    }

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum, Type securableClass)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullAndType ("methodNameEnum", methodNameEnum, typeof (Enum));
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      Enum enumValue = (Enum) methodNameEnum;

      CheckDeclaringTypeOfMethodNameEnum (enumValue, securableClass);

      MethodName = enumValue.ToString ();
      SecurableClass = securableClass;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodName = methodName;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      MethodName = methodName;
      SecurableClass = securableClass;
    }

    // methods and properties

    public new string ParameterName
    {
      get { return base.ParameterName; }
      set { base.ParameterName = value; }
    }
  }
}