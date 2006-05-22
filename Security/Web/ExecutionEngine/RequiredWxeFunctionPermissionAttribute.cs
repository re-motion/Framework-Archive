using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.Web.ExecutionEngine
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class RequiredWxeFunctionPermissionAttribute : Attribute
  {
    // types

    // static members

    // member fields

    private int? _parameterNumber;
    private string _parameterName;
    private string _protectedMethod;

    // construction and disposing

    public RequiredWxeFunctionPermissionAttribute (int parameterNumber, string protectedMethod)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("protectedMethod", protectedMethod);

      Initialize (parameterNumber, null, protectedMethod);
    }

    public RequiredWxeFunctionPermissionAttribute (string parameterName, string protectedMethod)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
      ArgumentUtility.CheckNotNullOrEmpty ("protectedMethod", protectedMethod);

      Initialize (null, parameterName, protectedMethod);
    }

    private void Initialize (int? parameterNumber, string parameterName, string protectedMethod)
    {
      _parameterNumber = parameterNumber;
      _parameterName = parameterName;
      _protectedMethod = protectedMethod;
    }

    // methods and properties


    public int? ParameterNumber
    {
      get { return _parameterNumber; }
    }


    public string ParameterName
    {
      get { return _parameterName; }
    }

    public string ProtectedMethod
    {
      get { return _protectedMethod; }
    }
	
  }
}