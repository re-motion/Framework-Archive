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
      :base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      
      MethodName = methodName;
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