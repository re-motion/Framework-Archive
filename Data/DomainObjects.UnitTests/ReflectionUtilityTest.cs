using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ReflectionUtilityTest: StandardMappingTest
  {
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
  }
}