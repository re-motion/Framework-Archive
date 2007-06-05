using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyFinderTest
  {
    [Test]
    public void Initialize ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithMixedProperties), true);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (ClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }
    
    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithMixedProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo.Length, Is.EqualTo (5));
      Assert.That (propertyInfo[0], Is.SameAs (GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "Boolean")));
      Assert.That (propertyInfo[1], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "Int32")));
      Assert.That (propertyInfo[2], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "String")));
      Assert.That (propertyInfo[3], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")));
      Assert.That (propertyInfo[4], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "PrivateString")));
    }

    [Test]
    public void FindPropertyInfos_ForDerivedClass ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithMixedProperties), false);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo.Length, Is.EqualTo (4));
      Assert.That (propertyInfo[0], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "Int32")));
      Assert.That (propertyInfo[1], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "String")));
      Assert.That (propertyInfo[2], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")));
      Assert.That (propertyInfo[3], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "PrivateString")));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithOneSideRelationProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos ();

      Assert.That (propertyInfo.Length, Is.EqualTo (0));
   }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The 'Rubicon.Data.DomainObjects.StorageClassNoneAttribute' is a mapping attribute and may only be applied at the property's base definiton.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute, "
        + "property: Int32")]
    public void FindPropertyInfos_ForDerivedClassHavingAnOverriddenPropertyWithMappingAttribute ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainWithErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.DerivedClassHavingAnOverriddenPropertyWithMappingAttribute",
          true,
          false);
      PropertyFinder propertyFinder = new PropertyFinder (type, false);
      
      propertyFinder.FindPropertyInfos();
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      return type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    }
  }
}
