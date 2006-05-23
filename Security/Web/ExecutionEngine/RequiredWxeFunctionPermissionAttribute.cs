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

    private Type _securableClass;	
    private int? _parameterNumber;
    private string _parameterName;
    private string _protectedMethod;

    // construction and disposing

    public RequiredWxeFunctionPermissionAttribute (int parameterNumber, string protectedMethod)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("protectedMethod", protectedMethod);

      Initialize (null, parameterNumber, null, protectedMethod);
    }

    public RequiredWxeFunctionPermissionAttribute (string parameterName, string protectedMethod)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
      ArgumentUtility.CheckNotNullOrEmpty ("protectedMethod", protectedMethod);

      Initialize (null, null, parameterName, protectedMethod);
    }

    public RequiredWxeFunctionPermissionAttribute (Type securableClass, string protectedMethod)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableType));
      ArgumentUtility.CheckNotNullOrEmpty ("protectedMethod", protectedMethod);

      Initialize (securableClass, null, null, protectedMethod);
    }

    public RequiredWxeFunctionPermissionAttribute (Type securableClass)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableType));

      Initialize (securableClass, null, null, null);
    }

    private void Initialize (Type securableClass, int? parameterNumber, string parameterName, string protectedMethod)
    {
      _securableClass = securableClass;
      _parameterNumber = parameterNumber;
      _parameterName = parameterName;
      _protectedMethod = protectedMethod;
    }

    // methods and properties

    public Type SecurableClass
    {
      get { return _securableClass; }
    }

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