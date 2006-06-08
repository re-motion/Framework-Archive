using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public class WxeDemandCreatePermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandCreatePermissionAttribute (Type securableClass)
      : base (MethodType.Constructor)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));
      SecurableClass = securableClass;
    }

    // methods and properties
  }
}