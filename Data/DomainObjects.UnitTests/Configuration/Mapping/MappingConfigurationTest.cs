using System;
using System.IO;
using NUnit.Framework;

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
  public void InitializeWithFileNames ()
  {
    try
    {
      MappingConfiguration.SetCurrent (new MappingConfiguration (@"mappingWithMinimumData.xml", @"mapping.xsd"));

      string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
      string schemaFile = Path.GetFullPath (@"mapping.xsd");

      Assert.AreEqual (configurationFile, MappingConfiguration.Current.ConfigurationFile);
      Assert.AreEqual (schemaFile, MappingConfiguration.Current.SchemaFile);
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
    ClassDefinition orderDefinition = new ClassDefinition ("Order", "Order", typeof (Order), "TestDomain");
    ClassDefinition orderTicketDefinition = new ClassDefinition ("OrderTicket", "OrderTicket", typeof (OrderTicket), "TestDomain"); 
    orderTicketDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID", false));

    VirtualRelationEndPointDefinition orderEndPointDefinition = new VirtualRelationEndPointDefinition (
        orderDefinition, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

    RelationEndPointDefinition orderTicketEndPointdefinition = new RelationEndPointDefinition (orderTicketDefinition, "Order", true);

    RelationDefinition relationDefinitionNotInMapping = new RelationDefinition (
        "RelationIDNotInMapping", 
        orderEndPointDefinition,
        orderTicketEndPointdefinition);

    Assert.IsFalse (MappingConfiguration.Current.Contains (orderEndPointDefinition));

  }
}
}
