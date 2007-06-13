using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class RelationPropertyFinderTest
  {
    [Test]
    public void Initialize ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (DerivedClassWithMixedProperties), true);

      Assert.That (propertyFinder.Type, Is.SameAs (typeof (DerivedClassWithMixedProperties)));
      Assert.That (propertyFinder.IncludeBaseProperties, Is.True);
    }

    [Test]
    public void FindPropertyInfos_ForClassWithMixedProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithMixedProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo.Length, Is.EqualTo (1));
      Assert.That (propertyInfo[0], Is.SameAs (GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithOneSideRelationProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo.Length, Is.EqualTo (4));
      Assert.That (propertyInfo[0], Is.SameAs (GetProperty (typeof (ClassWithOneSideRelationProperties), "NoAttribute")));
      Assert.That (propertyInfo[1], Is.SameAs (GetProperty (typeof (ClassWithOneSideRelationProperties), "NotNullable")));
      Assert.That (propertyInfo[2], Is.SameAs (GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToOne")));
      Assert.That (propertyInfo[3], Is.SameAs (GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToMany")));
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      return type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    }
  }
}