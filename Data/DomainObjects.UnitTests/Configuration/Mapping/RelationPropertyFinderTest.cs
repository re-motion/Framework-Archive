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

      Assert.That (
          propertyFinder.FindPropertyInfos(),
          Is.EqualTo (new PropertyInfo[] {GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne")}));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      RelationPropertyFinder propertyFinder = new RelationPropertyFinder (typeof (ClassWithOneSideRelationProperties), true);

      Assert.That (
          propertyFinder.FindPropertyInfos(),
          Is.EqualTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NoAttribute"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "NotNullable"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithOneSideRelationProperties), "BidirectionalOneToMany")
                  }));
    }

    private PropertyInfo GetProperty (Type type, string propertyName)
    {
      PropertyInfo propertyInfo =
          type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That (propertyInfo, Is.Not.Null, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }
  }
}