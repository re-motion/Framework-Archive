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

  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class WxeDemandMethodPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private MethodType _type;
    private Type _securableClass;	
    private string _parameterName;
    private string _method;

    // construction and disposing

    public WxeDemandMethodPermissionAttribute (MethodType type)
    {
      _type = type;
    }

    // methods and properties

    public MethodType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public Type SecurableClass
    {
      get { return _securableClass; }
      set
      {
        ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("SecurableClass", value, typeof (ISecurableType));
        _securableClass = value;
      }
    }

    public string ParameterName
    {
      get
      {
        return _parameterName; 
      }
      set 
      {
        ArgumentUtility.CheckNotNullOrEmpty ("ParameterName", value);
        _parameterName = value; 
      }
    }

    public string Method
    {
      get { return _method; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("Method", value);
        _method = value;
      }
    }
  }
}