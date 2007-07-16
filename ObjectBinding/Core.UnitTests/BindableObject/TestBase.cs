using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Security;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
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
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }
  }
}