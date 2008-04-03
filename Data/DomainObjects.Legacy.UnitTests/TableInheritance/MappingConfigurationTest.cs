using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance
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
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));

      MappingConfiguration mappingConfiguration = XmlBasedMappingConfiguration.Create ("DataDomainObjectsLegacy_MappingWithMinimumData.xml");
      mappingConfiguration.ClassDefinitions.Add (personClass);
      mappingConfiguration.Validate ();
    }

    [Test]
    public void SetCurrentValidates ()
    {
      XmlBasedClassDefinition personClass = new XmlBasedClassDefinition ("Person", null, c_testDomainProviderID, typeof (Person));

      MappingConfiguration mappingConfiguration = XmlBasedMappingConfiguration.Create ("DataDomainObjectsLegacy_MappingWithMinimumData.xml");
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
      MappingConfiguration mappingConfiguration = XmlBasedMappingConfiguration.Create ("DataDomainObjectsLegacy_TableInheritanceMapping.xml");
      ClassDefinition domainBaseClass = mappingConfiguration.ClassDefinitions.GetMandatory (typeof (DomainBase));
      Assert.IsNull (domainBaseClass.MyEntityName);
    }

    [Test]
    [ExpectedException (typeof (MappingException))]
    public void ConstructorValidates ()
    {
      XmlBasedMappingConfiguration.Create ("DataDomainObjectsLegacy_TableInheritanceMappingWithNonAbstractClassWithoutEntity.xml");
    }
  }
}
