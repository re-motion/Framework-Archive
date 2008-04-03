using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectBuilderTest : MappingBaseTest
  {
    [Test]
    public void BuildCompanyWithSerializableAttribute ()
    {
      XmlBasedClassDefinition companyClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Company");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (companyClass, DomainObjectBuilder.DefaultBaseClass, true, false);
        Assert.AreEqual (GetFile (@"Company.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCustomer () //TODO: WithMultilingualResourceAttribute
    {
      XmlBasedClassDefinition customerClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (customerClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"Customer.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildCeo ()
    {
      XmlBasedClassDefinition ceoClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (ceoClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"Ceo.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOfficial ()
    {
      XmlBasedClassDefinition officialClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Official");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (officialClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"Official.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrder ()
    {
      XmlBasedClassDefinition orderClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (orderClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"Order.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildOrderItem ()
    {
      XmlBasedClassDefinition orderItemClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (orderItemClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"OrderItem.cs"), writer.ToString ());
      }
    }

    [Test]
    public void BuildClassWithAllDataTypes ()
    {
      XmlBasedClassDefinition classWithAllDataTypesClass = (XmlBasedClassDefinition) MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithAllDataTypes");
      using (StringWriter writer = new StringWriter ())
      {
        DomainObjectBuilder builder = new DomainObjectBuilder (MappingConfiguration, writer);
        builder.Build (classWithAllDataTypesClass, DomainObjectBuilder.DefaultBaseClass, false, false);
        Assert.AreEqual (GetFile (@"ClassWithAllDataTypes.cs"), writer.ToString ());
      }
    }

    private string GetFile (string filename)
    {
      return ResourceManager.GetResourceString ("IntegrationTests.TestDomain." + filename);
    }
  }
}
