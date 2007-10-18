using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    [Test]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
          ReflectionUtility.GetPropertyName (propertyInfo));
    }

   
    private int TestProperty
    {
      get { return 0; }
      set { }
    }

    private static int StaticTestProperty
    {
      get { return 0; }
      set { }
    }

    public int TestPropertyMixedVisibility
    {
      get { return 12; }
      private set { }
    }

    [Test]
    public void GetPropertyFromMethod ()
    {
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName (""));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("bla"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("MethodWithLongName"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("get_"));
      Assert.IsNull (ReflectionUtility.GetPropertyNameForMethodName ("set_"));

      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("get_Prop"));
      Assert.AreEqual ("Prop", ReflectionUtility.GetPropertyNameForMethodName ("set_Prop"));

      Assert.IsNull (ReflectionUtility.GetPropertyForMethod (typeof (object).GetMethod ("ToString")));

      Assert.AreEqual (
          typeof (Order).GetProperty ("Number"),
          ReflectionUtility.GetPropertyForMethod (typeof (Order).GetMethod ("get_Number")));
      Assert.AreEqual (
          typeof (Order).GetProperty ("Number"),
          ReflectionUtility.GetPropertyForMethod (typeof (Order).GetMethod ("set_Number")));

      PropertyInfo privateProperty = typeof (ReflectionUtilityTest).GetProperty (
          "TestProperty",
          BindingFlags.NonPublic
          | BindingFlags.Instance);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetGetMethod (true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetSetMethod (true)));

      privateProperty = typeof (ReflectionUtilityTest).GetProperty (
          "StaticTestProperty",
          BindingFlags.NonPublic
          | BindingFlags.Static);
      Assert.IsNotNull (privateProperty);
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetGetMethod (true)));
      Assert.AreEqual (privateProperty, ReflectionUtility.GetPropertyForMethod (privateProperty.GetSetMethod (true)));

      PropertyInfo mixedVisibilityProperty = typeof (ReflectionUtilityTest).GetProperty (
          "TestPropertyMixedVisibility",
          BindingFlags.Public | BindingFlags.Instance);

      Assert.IsNotNull (mixedVisibilityProperty);
      Assert.AreEqual (mixedVisibilityProperty, ReflectionUtility.GetPropertyForMethod (mixedVisibilityProperty.GetGetMethod (true)));
      Assert.AreEqual (mixedVisibilityProperty, ReflectionUtility.GetPropertyForMethod (mixedVisibilityProperty.GetSetMethod (true)));
    }
 }
}