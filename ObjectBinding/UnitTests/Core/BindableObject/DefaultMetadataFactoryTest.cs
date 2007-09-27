using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DefaultMetadataFactoryTest
  {
    public class TestClass
    {
      public int Property { get { return 0; } }
    }

    [Test]
    public void CreatePropertyFinder ()
    {
      IPropertyFinder finder = DefaultMetadataFactory.Instance.CreatePropertyFinder (typeof (TestClass));
      Assert.AreSame (typeof (ReflectionBasedPropertyFinder), finder.GetType());
      Assert.AreSame (typeof (TestClass), new List<PropertyInfo> (finder.GetPropertyInfos ())[0].DeclaringType);
    }

    [Test]
    public void CreatePropertyReflector ()
    {
      PropertyInfo property = typeof (TestClass).GetProperty ("Property");
      PropertyReflector propertyReflector = DefaultMetadataFactory.Instance.CreatePropertyReflector (typeof (TestClass), property, BindableObjectProvider.Current);
      Assert.AreSame (typeof (PropertyReflector), propertyReflector.GetType ());
      Assert.AreSame (property, propertyReflector.PropertyInfo);
    }
  }
}