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

    public WxeDemandTargetStaticMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Static)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));
      
      MethodName = methodName;
      SecurableClass = securableClass;
    }

    // methods and properties
  }
}