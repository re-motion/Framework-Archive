using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Web.ExecutionEngine
{
  //[DemandTargetStaticMethodPermission (Akt.Methods.Protokollieren)] // default: containing type of nested enum -> Akt
  //[DemandTargetStaticMethodPermission (Akt.Methods.Protokollieren, typeof (Sachakt))]

  public class WxeDemandTargetStaticMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetStaticMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Static)
    {
      ArgumentUtility.CheckNotNullAndType ("methodNameEnum", methodNameEnum, typeof (Enum));

      Enum enumValue = (Enum) methodNameEnum;
      Type enumType = enumValue.GetType();

      // TODO: rewrite with test
      if (!typeof (ISecurableObject).IsAssignableFrom (enumType.DeclaringType))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' is not declared as a nested type or the declaring type does not implement interface '{1}'.", 
                enumType.FullName, 
                typeof (ISecurableObject).FullName),
            "methodNameEnum");
      }

      Initialize (enumValue.ToString (), enumType.DeclaringType);
    }

    public WxeDemandTargetStaticMethodPermissionAttribute (object methodNameEnum, Type securableClass)
      : base (MethodType.Static)
    {
      ArgumentUtility.CheckNotNullAndType ("methodNameEnum", methodNameEnum, typeof (Enum));
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      Enum enumValue = (Enum) methodNameEnum;
      Type enumType = enumValue.GetType ();

      // TODO: rewrite with test
      if (!typeof (ISecurableObject).IsAssignableFrom (enumType.DeclaringType))
      {
        throw new ArgumentException (string.Format (
                "Enumerated type '{0}' is not declared as a nested type or the declaring type does not implement interface '{1}'.",
                enumType.FullName,
                typeof (ISecurableObject).FullName),
            "methodNameEnum");
      }

      // TODO: rewrite with test
      if (!enumType.DeclaringType.IsAssignableFrom (securableClass))
      {
        throw new ArgumentException (string.Format (
                "Type '{0}' cannot be assigned to the declaring type '{1}' of the enum value specified by parameter 'methodNameEnum'.",
                securableClass,
                enumType.DeclaringType),
            "securableClass");
      }

      Initialize (enumValue.ToString (), securableClass);
    }

    public WxeDemandTargetStaticMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Static)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      Initialize (methodName, securableClass);
    }

    // methods and properties

    private void Initialize (string methodName, Type securableClass)
    {
      SecurableClass = securableClass;
      MethodName = methodName;
    }
  }
}