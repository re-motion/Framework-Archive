using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Rubicon.Data.DomainObjects.Legacy.Mapping;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectBuilderTest ()
    {
    }

    // methods and properties

    [Test]
    public void BuildCompanyWithSerializableAttribute ()
    {
      XmlBasedClassDefinition companyClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Company");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, companyClass, DomainObjectBuilder.DefaultBaseClass, true, false);
        Assert.AreEqual (File.ReadAllText (@"Company.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCustomer () //TODO: WithMultilingualResourceAttribute
    {
      XmlBasedClassDefinition customerClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, customerClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Customer.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCeo ()
    {
      XmlBasedClassDefinition ceoClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, ceoClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Ceo.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOfficial ()
    {
      XmlBasedClassDefinition officialClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Official");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, officialClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Official.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrder ()
    {
      XmlBasedClassDefinition orderClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, orderClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Order.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrderItem ()
    {
      XmlBasedClassDefinition orderItemClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, orderItemClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"OrderItem.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildClassWithAllDataTypes ()
    {
      XmlBasedClassDefinition classWithAllDataTypesClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithAllDataTypes");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, classWithAllDataTypesClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"ClassWithAllDataTypes.cs"), writer.ToString ());
      }
    }
  }
}
