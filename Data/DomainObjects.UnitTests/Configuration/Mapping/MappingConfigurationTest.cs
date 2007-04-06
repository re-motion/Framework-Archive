using System;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class MappingConfigurationTest : LegacyMappingTest
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
    [Ignore]
    public void InitializeWithFileNamesOnly ()
    {
      MappingConfiguration configuration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"MappingWithMinimumData.xml");

      string configurationFile = Path.GetFullPath (@"MappingWithMinimumData.xml");

      //Assert.IsNotNull (configuration.Loader);
      //Assert.AreEqual (configurationFile, ((MappingLoader) configuration.Loader).ConfigurationFile);
      //Assert.IsTrue (configuration.ResolveTypes);
    }

    [Test]
    [Ignore]
    public void InitializeWithFileNamesAndResolveTypes ()
    {
      MappingConfiguration configuration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"MappingWithMinimumData.xml", true);

      string configurationFile = Path.GetFullPath (@"MappingWithMinimumData.xml");

      //Assert.IsNotNull (configuration.Loader);
      //Assert.AreEqual (configurationFile, ((MappingLoader) configuration.Loader).ConfigurationFile);
      //Assert.IsTrue (configuration.ResolveTypes);
    }

    [Test]
    [Ignore]
    public void InitializeWithLoaderAndResolveTypes ()
    {
      //MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"MappingWithMinimumData.xml", true));

      //string configurationFile = Path.GetFullPath (@"MappingWithMinimumData.xml");

      //Assert.IsNotNull (configuration.Loader);
      //Assert.AreEqual (configurationFile, ((MappingLoader) configuration.Loader).ConfigurationFile);
      //Assert.IsTrue (configuration.ResolveTypes);
    }

    [Test]
    [Ignore]
    public void SetCurrent ()
    {
      try
      {
        //MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"MappingWithMinimumData.xml", true));
        //MappingConfiguration.SetCurrent (configuration);

        //Assert.AreSame (configuration, MappingConfiguration.Current);
      }
      finally
      {
        MappingConfiguration.SetCurrent (null);
      }
    }

    [Test]
    [Ignore]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Argument 'mappingConfiguration' must have property 'ResolveTypes' set.\r\nParameter name: mappingConfiguration")]
    public void SetCurrentRejectsUnresolvedTypes ()
    {
      //MappingConfiguration configuration = MappingConfiguration.CreateConfigurationFromFileBasedLoader(@"MappingWithMinimumData.xml", false);
      //MappingConfiguration.SetCurrent (configuration);
    }

    [Test]
    [Ignore]
    public void ApplicationName ()
    {
      //Assert.AreEqual ("UnitTests", ((MappingLoader)MappingConfiguration.Current.Loader).GetApplicationName());
    }

    [Test]
    public void ContainsClassDefinition ()
    {
      Assert.IsFalse (MappingConfiguration.Current.Contains (LegacyTestMappingConfiguration.Current.ClassDefinitions["Order"]));
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
      Assert.IsFalse (MappingConfiguration.Current.Contains (LegacyTestMappingConfiguration.Current.ClassDefinitions["Order"]["OrderNumber"]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.ClassDefinitions["Order"]["OrderNumber"]));
    }

    [Test]
    public void ContainsRelationDefinition ()
    {

      Assert.IsFalse (MappingConfiguration.Current.Contains (LegacyTestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"]));
    }

    [Test]
    public void ContainsRelationEndPointDefinition ()
    {
      Assert.IsFalse (MappingConfiguration.Current.Contains (LegacyTestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[0]));
      Assert.IsFalse (MappingConfiguration.Current.Contains (LegacyTestMappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[1]));

      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[0]));
      Assert.IsTrue (MappingConfiguration.Current.Contains (MappingConfiguration.Current.RelationDefinitions["OrderToOrderItem"].EndPointDefinitions[1]));
    }

    [Test]
    public void ContainsRelationEndPointDefinitionNotInMapping ()
    {
      ReflectionBasedClassDefinition orderDefinition = new ReflectionBasedClassDefinition ("Order", "Order", "TestDomain", typeof (Order));
      ReflectionBasedClassDefinition orderTicketDefinition = new ReflectionBasedClassDefinition ("OrderTicket", "OrderTicket", "TestDomain", typeof (OrderTicket));
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
    [Ignore]
    public void MappingWithUnresolvedTypes ()
    {
      //MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"MappingWithUnresolvedTypes.xml", false));

      //Assert.IsFalse (configuration.ClassDefinitions.AreResolvedTypesRequired);
    }

    [Test]
    [Ignore]
    public void EntireMappingWithUnresolvedTypes ()
    {
      //MappingConfiguration configuration = new MappingConfiguration (new MappingLoader (@"EntireMappingWithUnresolvedTypes.xml", false));

      //Assert.IsNotNull (configuration.Loader);
      //Assert.IsFalse (configuration.Loader.ResolveTypes);
      //Assert.IsFalse (configuration.ClassDefinitions.AreResolvedTypesRequired);

      //foreach (ClassDefinition classDefinition in configuration.ClassDefinitions)
      //{
      //  string classMessage = "Class: " + classDefinition.ID;
      //  Assert.IsNull (classDefinition.ClassType, classMessage);
      //  Assert.IsNotNull (classDefinition.ClassTypeName, classMessage);
      //  Assert.IsFalse (classDefinition.IsClassTypeResolved, classMessage);

      //  foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
      //  {
      //    string propertyMessage = classMessage + ", Property: " + propertyDefinition.PropertyName;
      //    Assert.IsNull (propertyDefinition.PropertyType, propertyMessage);
      //    Assert.IsNotNull (propertyDefinition.MappingTypeName, propertyMessage);
      //    Assert.IsFalse (propertyDefinition.IsPropertyTypeResolved, propertyMessage);
      //  }
      //}

      //foreach (RelationDefinition relationDefinition in configuration.RelationDefinitions)
      //{
      //  foreach (IRelationEndPointDefinition endPoint in relationDefinition.EndPointDefinitions)
      //  {
      //    string endPointMessage = "Relation: " + relationDefinition.ID + ", PropertyName: " + endPoint.PropertyName;
      //    Assert.IsNull (endPoint.PropertyType, endPointMessage);

      //    if (endPoint.IsNull)
      //      Assert.IsNull (endPoint.PropertyTypeName, endPointMessage);
      //    else
      //      Assert.IsNotNull (endPoint.PropertyTypeName, endPointMessage);

      //    Assert.IsFalse (endPoint.IsPropertyTypeResolved, endPointMessage);
      //  }
      //}
    }
  }
}
