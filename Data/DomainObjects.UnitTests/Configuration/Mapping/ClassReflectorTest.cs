using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class ClassReflectorTest: StandardMappingTest
  {
    private ClassDefinitionChecker _classDefinitionChecker;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinitionChecker = new ClassDefinitionChecker();
    }

    [Test]
    [Ignore()]
    public void GetMetadata()
    {
      ClassReflector classReflector = new ClassReflector();
      ClassDefinition expected = TestMappingConfiguration.Current.ClassDefinitions[typeof (FileSystemItem)];

      ClassDefinition actual = classReflector.GetMetadata (typeof (FileSystemItem));

      Assert.IsNotNull (actual);
      _classDefinitionChecker.Check (expected, actual);
    }
  }
}