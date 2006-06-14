using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class MappingConfigurationTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public MappingConfigurationTest ()
    {
    }

    // methods and properties

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void Validate ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));

      MappingConfiguration mappingConfiguration = new MappingConfiguration ("MappingWithMinimumData.xml");
      mappingConfiguration.ClassDefinitions.Add (personClass);
      mappingConfiguration.Validate ();
    }

    [Test]
    public void SetCurrentValidates ()
    {
      ClassDefinition personClass = new ClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));

      MappingConfiguration mappingConfiguration = new MappingConfiguration ("MappingWithMinimumData.xml");
      mappingConfiguration.ClassDefinitions.Add (personClass);

      try
      {
        MappingConfiguration.SetCurrent (mappingConfiguration);
        Assert.Fail ("ArgumentException was expected.");
      }
      catch (ArgumentException ex)
      {
        Assert.AreNotSame (mappingConfiguration, MappingConfiguration.Current);
        Assert.IsInstanceOfType (typeof (MappingException), ex.InnerException);

        string expectedMessage = string.Format (
            "The specified MappingConfiguration is invalid due to the following reason: '{0}'.\r\nParameter name: mappingConfiguration",
            ex.InnerException.Message);
      }
    }

    [Test]
    public void TableInheritanceMapping ()
    {
      MappingConfiguration mappingConfiguration = new MappingConfiguration ("TableInheritanceMapping.xml");
      ClassDefinition domainBaseClass = mappingConfiguration.ClassDefinitions.GetMandatory (typeof (DomainBase));
      Assert.IsNull (domainBaseClass.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void ConstructorValidates ()
    {
      new MappingConfiguration ("TableInheritanceMappingWithNonAbstractClassWithoutEntity.xml");
    }
  }
}
