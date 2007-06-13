using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
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
    public void FindPropertyInfos_ForClassWithMixedProperties ()
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
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithOneSideRelationProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo.Length, Is.EqualTo (0));
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      return type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    }
  }
}