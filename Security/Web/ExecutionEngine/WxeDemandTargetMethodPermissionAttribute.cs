using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.ComponentModel;

namespace Rubicon.Security.Web.ExecutionEngine
{
  //[DemandTargetMethodPermission (Akt.Methods.Protokollieren)] // default: 1st parameter
  // optional/nie: [WxeDemandTargetMethodPermission (Akt.Methods.Protokollieren, ParameterName = "obj")]

  public class WxeDemandTargetMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetMethodPermissionAttribute (string methodName)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodName = methodName;
    }

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullAndType ("methodNameEnum", methodNameEnum, typeof (Enum));

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

      SecurableClass = enumType.DeclaringType;
      MethodName = enumValue.ToString ();
    }

    // methods and properties

    // TODO: Property is to be removed 
    [EditorBrowsable (EditorBrowsableState.Never)]
    public new string ParameterName
    {
      get { return base.ParameterName; }
      set { base.ParameterName = value; }
    }
  }
}