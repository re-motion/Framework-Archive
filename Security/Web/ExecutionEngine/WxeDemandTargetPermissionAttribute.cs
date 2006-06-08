using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Web.ExecutionEngine
{
  public enum MethodType
  {
    Instance,
    Static,
    Constructor
  }

  //// dirty - optional, irgendwann, vielleicht doch -> nie
  //[WxeDemandObjectPermission (AccessTypes.Edit)] // default: 1st parameter
  //[WxeDemandObjectPermission (AccessTypes.Edit, ParameterName = "obj")]
  //[WxeDemandClassPermission (AccessTypes.Search, typeof (Akt))]

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class WxeDemandTargetPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private MethodType _methodType;
    private Type _securableClass;
    private string _parameterName;
    private string _methodName;

    // construction and disposing

    public WxeDemandTargetPermissionAttribute (MethodType type)
    {
      _methodType = type;
    }

    // methods and properties

    public MethodType MethodType
    {
      get { return _methodType; }
    }

    public Type SecurableClass
    {
      get
      {
        return _securableClass;
      }
      protected set
      {
        ArgumentUtility.CheckTypeIsAssignableFrom ("SecurableClass", value, typeof (ISecurableObject));
        _securableClass = value;
      }
    }

    public string ParameterName
    {
      get { return _parameterName; }
      protected set { _parameterName = value; }
    }

    public string MethodName
    {
      get { return _methodName; }
      protected set { _methodName = value; }
    }
  }
}