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

      Assert.That (
          propertyFinder.FindPropertyInfos(),
          Is.EqualTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (ClassWithMixedPropertiesNotInMapping), "Boolean"),
                      GetProperty (typeof (ClassWithMixedProperties), "Int32"),
                      GetProperty (typeof (ClassWithMixedProperties), "String"),
                      GetProperty (typeof (ClassWithMixedProperties), "UnidirectionalOneToOne"),
                      GetProperty (typeof (ClassWithMixedProperties), "PrivateString")
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForClassWithOneSideRelationProperties ()
    {
      PropertyFinder propertyFinder = new PropertyFinder (typeof (ClassWithOneSideRelationProperties), true);

      PropertyInfo[] propertyInfo = propertyFinder.FindPropertyInfos();

      Assert.That (propertyInfo, Is.Empty);
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