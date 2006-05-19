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
  public class MappingConfigurationTest : StandardMappingTest
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
    public void InitializeWithFileNamesOnly ()
    {
      MappingConfiguration configuration = new MappingConfiguration (@"mappingWithMinimumData.xml", @"mapping.xsd");

      string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
      string schemaFile = Path.GetFullPath (@"mapping.xsd");

      Assert.AreEqual (configurationFile, configuration.ConfigurationFile);
      Assert.AreEqual (schemaFile, configuration.SchemaFile);
      Assert.IsTrue (configuration.ResolveTypes);
    }

    [Test]
    public void InitializeWithFileNamesAndResolveTypes ()
    {
      MappingConfiguration configuration = new MappingConfiguration (@"mappingWithMinimumData.xml", @"mapping.xsd", true);

      string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
      string schemaFile = Path.GetFullPath (@"mapping.xsd");

      Assert.AreEqual (configurationFile, configuration.ConfigurationFile);
      Assert.AreEqual (schemaFile, configuration.SchemaFile);
      Assert.IsTrue (configuration.ResolveTypes);
    }

    [Test]
    public void InitializeWithLoaderAndResolveTypes ()
    {
      MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"mappingWithMinimumData.xml", @"mapping.xsd", true));

      string configurationFile = Path.GetFullPath (@"mappingWithMinimumData.xml");
      string schemaFile = Path.GetFullPath (@"mapping.xsd");

      Assert.AreEqual (configurationFile, configuration.ConfigurationFile);
      Assert.AreEqual (schemaFile, configuration.SchemaFile);
      Assert.IsTrue (configuration.ResolveTypes);
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
    [ExpectedException (typeof (ArgumentException), 
        "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes ()
    {
      MappingConfiguration configuration = new MappingConfiguration (@"mappingWithMinimumData.xml", @"mapping.xsd", false);
      MappingConfiguration.SetCurrent (configuration);
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
    public void MappingWithUnresolvedTypes ()
    {
      MappingConfiguration configuration = new MappingConfiguration (
          new MappingLoader (@"mappingWithUnresolvedTypes.xml", @"mapping.xsd", false));

      Assert.IsFalse (configuration.ClassDefinitions.AreResolvedTypesRequired);
    }

    [Test]
    public void EntireMappingWithUnresolvedTypes ()
    {
      MappingConfiguration configuration = new MappingConfiguration (
          new MappingLoader (@"entireMappingWithUnresolvedTypes.xml", @"mapping.xsd", false));

      Assert.IsFalse (configuration.ResolveTypes);
      Assert.IsFalse (configuration.ClassDefinitions.AreResolvedTypesRequired);

      foreach (ClassDefinition classDefinition in configuration.ClassDefinitions)
      {
        string classMessage = "Class: " + classDefinition.ID;
        Assert.IsNull (classDefinition.ClassType, classMessage);
        Assert.IsNotNull (classDefinition.ClassTypeName, classMessage);
        Assert.IsFalse (classDefinition.IsClassTypeResolved, classMessage);

        foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          string propertyMessage = classMessage + ", Property: " + propertyDefinition.PropertyName;
          Assert.IsNull (propertyDefinition.PropertyType, propertyMessage);
          Assert.IsNotNull (propertyDefinition.MappingTypeName, propertyMessage);
          Assert.IsFalse (propertyDefinition.IsPropertyTypeResolved, propertyMessage);
        }
      }

      foreach (RelationDefinition relationDefinition in configuration.RelationDefinitions)
      {
        foreach (IRelationEndPointDefinition endPoint in relationDefinition.EndPointDefinitions)
        {
          string endPointMessage = "Relation: " + relationDefinition.ID + ", PropertyName: " + endPoint.PropertyName;
          Assert.IsNull (endPoint.PropertyType, endPointMessage);

          if (endPoint.IsNull)
            Assert.IsNull (endPoint.PropertyTypeName, endPointMessage);
          else
            Assert.IsNotNull (endPoint.PropertyTypeName, endPointMessage);

          Assert.IsFalse (endPoint.IsPropertyTypeResolved, endPointMessage);
        }
      }
    }
  }
}
