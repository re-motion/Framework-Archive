using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    private class DerivedObjectList : ObjectList<DomainObject>
    {
    }

    private class GenericDerivedObjectList<T> : ObjectList<DomainObject>
        where T: struct
    {
    }

    private class DerivedObjectList<T> : ObjectList<T>
        where T: DomainObject
    {
    }

    [Test]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("Int32");

      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
          ReflectionUtility.GetPropertyName (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithMixedProperties> ("String");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("OtherString");

      Assert.AreSame (
          typeof (DerivedClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForNewPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("String");

      Assert.AreSame (
          typeof (DerivedClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnBaseClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<ClassWithMixedProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    [Test]
    public void GetOriginalDeclaringType_ForOverriddenPropertyOnDerivedClass ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo<DerivedClassWithMixedProperties> ("Int32");

      Assert.AreSame (
          typeof (ClassWithMixedProperties),
          ReflectionUtility.GetOriginalDeclaringType (propertyInfo));
    }

    protected PropertyInfo GetPropertyInfo<T> (string property)
    {
      return typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
    }

    [Test]
    public void IsPropertyAccessor ()
    {
      Assert.IsFalse (ReflectionUtility.IsPropertyAccessor (typeof (object).GetConstructor (Type.EmptyTypes)));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (object).GetConstructor (Type.EmptyTypes)));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (object).GetConstructor (Type.EmptyTypes)));

      Assert.IsFalse (ReflectionUtility.IsPropertyAccessor (typeof (object).GetMethod ("ToString")));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (object).GetMethod ("ToString")));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (object).GetMethod ("ToString")));

      Assert.IsTrue (ReflectionUtility.IsPropertyAccessor (typeof (Order).GetMethod ("get_Number")));
      Assert.IsTrue (ReflectionUtility.IsPropertyAccessor (typeof (Order).GetMethod ("set_Number")));
      Assert.IsTrue (ReflectionUtility.IsPropertyGetter (typeof (Order).GetMethod ("get_Number")));
      Assert.IsFalse (ReflectionUtility.IsPropertySetter (typeof (Order).GetMethod ("get_Number")));
      Assert.IsFalse (ReflectionUtility.IsPropertyGetter (typeof (Order).GetMethod ("set_Number")));
      Assert.IsTrue (ReflectionUtility.IsPropertySetter (typeof (Order).GetMethod ("set_Number")));
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

    [Test]
    public void IsObjectList_WithClosedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (ObjectList<DomainObject>)));
    }

    [Test]
    public void IsObjectList_WithOpenObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (ObjectList<>)));
    }

    [Test]
    public void IsObjectList_WithClosedDerivedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (DerivedObjectList<DomainObject>)));
    }

    [Test]
    public void IsObjectList_WithOpenDerivedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (DerivedObjectList<>)));
    }

    [Test]
    public void IsObjectList_WithNonGenericDerivedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (DerivedObjectList)));
    }

    [Test]
    public void IsObjectList_WithClosedGenericDerivedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (GenericDerivedObjectList<int>)));
    }

    [Test]
    public void IsObjectList_WithOpenGenericDerivedObjectList ()
    {
      Assert.IsTrue (ReflectionUtility.IsObjectList (typeof (GenericDerivedObjectList<>)));
    }

    [Test]
    public void IsObjectList_WithDomainObjectCollection ()
    {
      Assert.IsFalse (ReflectionUtility.IsObjectList (typeof (DomainObjectCollection)));
    }


    [Test]
    public void GetObjectListTypeParameter_WithClosedObjectList ()
    {
      Assert.AreSame (typeof (DomainObject), ReflectionUtility.GetObjectListTypeParameter (typeof (ObjectList<DomainObject>)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithOpenObjectList ()
    {
      Assert.IsNull (ReflectionUtility.GetObjectListTypeParameter (typeof (ObjectList<>)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithClosedDerivedObjectList ()
    {
      Assert.AreSame (typeof (DomainObject), ReflectionUtility.GetObjectListTypeParameter (typeof (DerivedObjectList<DomainObject>)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithOpenDerivedObjectList ()
    {
      Assert.IsNull (ReflectionUtility.GetObjectListTypeParameter (typeof (DerivedObjectList<>)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithNonGenericDerivedObjectList ()
    {
      Assert.AreSame (typeof (DomainObject), ReflectionUtility.GetObjectListTypeParameter (typeof (DerivedObjectList)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithClosedGenericDerivedObjectList ()
    {
      Assert.AreSame (typeof (DomainObject), ReflectionUtility.GetObjectListTypeParameter (typeof (GenericDerivedObjectList<int>)));
    }

    [Test]
    public void GetObjectListTypeParameter_WithOpenGenericDerivedObjectList ()
    {
      Assert.AreSame (typeof (DomainObject), ReflectionUtility.GetObjectListTypeParameter (typeof (GenericDerivedObjectList<>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage =
        "Argument type has type Rubicon.Data.DomainObjects.DomainObjectCollection when type Rubicon.Data.DomainObjects.ObjectList`1[T] was expected.\r\n"
        + "Parameter name: type")]
    public void GetObjectListTypeParameter_WithDomainObjectCollection ()
    {
      ReflectionUtility.GetObjectListTypeParameter (typeof (DomainObjectCollection));
    }
  }
}