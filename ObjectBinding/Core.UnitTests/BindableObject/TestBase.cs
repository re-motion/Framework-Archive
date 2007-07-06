using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Security;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  public class TestBase
  {
    [SetUp]
    public virtual void SetUp ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
    }

    protected PropertyInfo GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }
  }
}