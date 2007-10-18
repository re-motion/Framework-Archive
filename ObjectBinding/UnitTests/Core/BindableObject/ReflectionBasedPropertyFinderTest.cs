using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.ObjectBinding.BindableObject;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ReflectionBasedPropertyFinderTest
  {
    public class BaseTestType
    {
      public static int BasePublicStaticProperty { get { return 0; } }
      protected static int BaseProtectedStaticProperty { get { return 0; } }

      public virtual int BasePublicInstanceProperty { get { return 0; } }
      protected int BaseProtectedInstanceProperty { get { return 0; } }
    }

    public class TestType : BaseTestType
    {
      public static int PublicStaticProperty { get { return 0; } }
      protected static int ProtectedStaticProperty { get { return 0; } }

      public int PublicInstanceProperty { get { return 0; } }
      protected int ProtectedInstanceProperty { get { return 0; } }
    }

    [Test]
    public void ReturnsPublicInstancePropertiesFromThisAndBase ()
    {
      ReflectionBasedPropertyFinder finder = new ReflectionBasedPropertyFinder (typeof (TestType));
      List<PropertyInfo> properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestType).GetProperty ("PublicInstanceProperty"),
                      typeof (BaseTestType).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    public class TestTypeHidingProperties : TestType
    {
      public override int BasePublicInstanceProperty { get { return 1; } }
      public new int PublicInstanceProperty { get { return 1; } }
    }

    [Test]
    public void IgnoresBasePropertiesWithSameName ()
    {
      ReflectionBasedPropertyFinder finder = new ReflectionBasedPropertyFinder (typeof (TestTypeHidingProperties));
      List<PropertyInfo> properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (
          properties,
          Is.EquivalentTo (
              new object[]
                  {
                      typeof (TestTypeHidingProperties).GetProperty ("PublicInstanceProperty"),
                      typeof (TestTypeHidingProperties).GetProperty ("BasePublicInstanceProperty")
                  }));
    }

    public interface ITestInterface
    {
      int InterfaceProperty { get; }
    }

    public interface IExplicitTestInterface
    {
      int InterfaceProperty { get; }
    }

    public class TestTypeWithInterfaces : ITestInterface, IExplicitTestInterface
    {
      public int InterfaceProperty { get { return 0; } }
      int IExplicitTestInterface.InterfaceProperty { get { return 0; } }
    }

    public class DerivedTypeWithInterfaces : TestTypeWithInterfaces
    {
    }

    [Test]
    public void FindsPropertiesFromImplicitInterfaceImplementations ()
    {
      ReflectionBasedPropertyFinder finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      List<PropertyInfo> properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty ("InterfaceProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementations ()
    {
      ReflectionBasedPropertyFinder finder = new ReflectionBasedPropertyFinder (typeof (TestTypeWithInterfaces));
      List<PropertyInfo> properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (
          typeof (ReflectionBasedPropertyFinderTest).FullName + ".IExplicitTestInterface.InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void FindsPropertiesFromExplicitInterfaceImplementationsOnBase ()
    {
      ReflectionBasedPropertyFinder finder = new ReflectionBasedPropertyFinder (typeof (DerivedTypeWithInterfaces));
      List<PropertyInfo> properties = new List<PropertyInfo> (PropertyInfoAdapter.UnwrapCollection (finder.GetPropertyInfos ()));
      Assert.That (properties,
          List.Contains (typeof (TestTypeWithInterfaces).GetProperty (
          typeof (ReflectionBasedPropertyFinderTest).FullName + ".IExplicitTestInterface.InterfaceProperty",
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void NoPropertiesFromBindableObjectMixins ()
    {
      Type targetType = typeof (ClassWithIdentity);
      Type concreteType = Mixins.TypeUtility.GetConcreteType (targetType);

      List<IPropertyInformation> targetTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (targetType).GetPropertyInfos ());
      List<IPropertyInformation> concreteTypeProperties = new List<IPropertyInformation> (new ReflectionBasedPropertyFinder (concreteType).GetPropertyInfos ());

      Assert.That (concreteTypeProperties, Is.EquivalentTo (targetTypeProperties));
    }
  }
}