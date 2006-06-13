using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
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
      ClassDefinition companyClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Company");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, companyClass, DomainObjectBuilder.DefaultBaseClass, true, false);
        Assert.AreEqual (File.ReadAllText (@"Company.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCustomer () //TODO: WithMultilingualResourceAttribute
    {
      ClassDefinition customerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, customerClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Customer.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCeo ()
    {
      ClassDefinition ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, ceoClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Ceo.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOfficial ()
    {
      ClassDefinition officialClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Official");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, officialClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Official.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrder ()
    {
      ClassDefinition orderClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, orderClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"Order.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrderItem ()
    {
      ClassDefinition orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, orderItemClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"OrderItem.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildClassWithAllDataTypes ()
    {
      ClassDefinition classWithAllDataTypesClass = MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithAllDataTypes");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder.Build (MappingConfiguration, writer, classWithAllDataTypesClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (File.ReadAllText (@"ClassWithAllDataTypes.cs"), writer.ToString ());
      }
    }
  }
}
