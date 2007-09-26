using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Security;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  public class TestBase
  {
    [SetUp]
    public virtual void SetUp ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      BindableObjectProvider.SetCurrent (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      BindableObjectProvider.SetCurrent (null);      
    }

    protected PropertyInfo GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance );
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }

    protected bool IsNullable (PropertyReflector reflector)
    {
      return (bool) PrivateInvoke.InvokeNonPublicMethod (reflector, "GetIsNullable");
    }

    protected Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (reflector, "GetUnderlyingType");
    }

    protected PropertyBase.Parameters GetPropertyParameters (PropertyInfo property, BindableObjectProvider provider)
    {
      PropertyReflector reflector = new PropertyReflector (property, provider);
      return (PropertyBase.Parameters) PrivateInvoke.InvokeNonPublicMethod (reflector, "CreateParameters", GetUnderlyingType (reflector));
    }
  }
}