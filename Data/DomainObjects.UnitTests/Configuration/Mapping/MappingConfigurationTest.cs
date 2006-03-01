using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class MappingConfigurationTest
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
  public void InitializeWithFileNamesAndResolveTypeNames ()
  {
    MappingConfiguration configuration = new MappingConfiguration (@"mappingWithMinimumData.xml", @"mapping.xsd", true);

    string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
    string schemaFile = Path.GetFullPath (@"mapping.xsd");

    Assert.AreEqual (configurationFile, configuration.ConfigurationFile);
    Assert.AreEqual (schemaFile, configuration.SchemaFile);
    Assert.IsTrue (configuration.ResolveTypeNames);
  }

  [Test]
  public void InitializeWithLoaderAndResolveTypeNames ()
  {
    MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"mappingWithMinimumData.xml", @"mapping.xsd", true));
    
    string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
    string schemaFile = Path.GetFullPath (@"mapping.xsd");

    Assert.AreEqual (configurationFile, configuration.ConfigurationFile);
    Assert.AreEqual (schemaFile, configuration.SchemaFile);
    Assert.IsTrue (configuration.ResolveTypeNames);
  }

  [Test]
  public void SetCurrent ()
  {
    try
    {
      MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"mappingWithMinimumData.xml", @"mapping.xsd", true));
      MappingConfiguration.SetCurrent (configuration);

      Assert.AreSame (configuration, MappingConfiguration.Current);
    }
    finally
    {
      MappingConfiguration.SetCurrent (null);
    }
  }

  [Test]
  public void ApplicationName ()
  {
    Assert.AreEqual ("UnitTests", MappingConfiguration.Current.ApplicationName);
  }

  [Test]
  public void ContainsClassDefinition ()
  {
    Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.ClassDefinitions["Order"]));
    Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions["Order"]));
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ContainsNull ()
  {
    MappingConfiguration.Current.Contains ((ClassDefinition) null);
  }

  [Test]
  public void ContainsPropertyDefinition ()
  {
    Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.ClassDefinitions["Order"]["OrderNumber"]));
    Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions["Order"]["OrderNumber"]));
  }

  [Test]
  public void ContainsRelationDefinition ()
  {

    Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"]));
    Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"]));
  }

  [Test]
  public void ContainsRelationEndPointDefinition ()
  {
    Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[0]));
    Assert.IsFalse (MappingConfiguration.Current.Contains (TestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[1]));

    Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[0]));
    Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[1]));
  }

  [Test]
  public void ContainsRelationEndPointDefinitionNotInMapping ()
  {
    ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
    ClassDefinition orderTicketDefinition = new ClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket)); 
    orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, false));

    VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
        orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

    RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

    RelationDefinition relationDefinitionNotInMapping = new RelationDefinition (
        "RelationIDNotInMapping", 
        orderEndPointDefinition,
        orderTicketEndPointdefinition);

    Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));
  }

  [Test]
  public void MappingWithUnresolvedTypeNames ()
  {
    MappingConfiguration configuration = new MappingConfiguration (
        new MappingLoader (@"mappingWithUnresolvedTypes.xml", @"mapping.xsd", false));

    Assert.IsFalse (configuration.ClassDefinitions.AreResolvedTypeNamesRequired);
    Assert.AreEqual (1, configuration.ClassDefinitions.Count);
    
    ClassDefinition classDefinition = configuration.ClassDefinitions.GetMandatory ("ClassWithUnresolvedTypes");
    Assert.IsFalse (classDefinition.IsClassTypeResolved);
    Assert.AreEqual ("UnknownClassType, Rubicon.Data.DomainObjects.UnitTests", classDefinition.ClassTypeName);

    PropertyDefinitionCollection propertyDefinitions = classDefinition.GetPropertyDefinitions();
    Assert.AreEqual (2, propertyDefinitions.Count);

    PropertyDefinition int32Property = propertyDefinitions["Int32Property"];
    Assert.IsFalse (int32Property.IsPropertyTypeResolved);
    Assert.AreEqual ("int32", int32Property.MappingTypeName);

    PropertyDefinition enumProperty = propertyDefinitions["EnumProperty"];
    Assert.IsFalse (enumProperty.IsPropertyTypeResolved);
    Assert.AreEqual ("UnknownClassType+EnumType, Rubicon.Data.DomainObjects.UnitTests", enumProperty.MappingTypeName);

    Assert.AreEqual (0, configuration.RelationDefinitions.Count);
  }
}

}
