using System;
using System.Data.SqlClient;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderLoadDataContainerTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderLoadDataContainerTest ()
  {
  }

  // methods and properties

  [Test]
  public void LoadDataContainerWithGuidID ()
  {
    ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

    DataContainer container = Provider.LoadDataContainer (id);

    Assert.IsNotNull (container);
    Assert.AreEqual (container.ID, id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), "Error while executing SQL command.")]
  public void LoadDataContainerWithInvalidIDType ()
  {
    ObjectID id = new ObjectID ("ClassWithKeyOfInvalidType", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

    try
    {
      DataContainer container = Provider.LoadDataContainer (id);
    }
    catch (StorageProviderException e)
    {
      Assert.AreEqual (typeof (SqlException), e.InnerException.GetType ());
      throw e;
    }
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), "Error while executing SQL command.")]
  public void LoadDataContainerWithoutIDColumn ()
  {
    ObjectID id = new ObjectID ("ClassWithoutIDProperty", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

    try
    {
      DataContainer container = Provider.LoadDataContainer (id);
    }
    catch (StorageProviderException e)
    {
      Assert.AreEqual (typeof (SqlException), e.InnerException.GetType ());
      throw e;
    }
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), "The mandatory column 'ClassID' could not be found.")]
  public void LoadDataContainerWithoutClassIDColumn ()
  {
    ObjectID id = new ObjectID ("ClassWithoutClassIDProperty", new Guid ("{DDD02092-355B-4820-90B6-7F1540C0547E}"));

    DataContainer container = Provider.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), "The mandatory column 'Timestamp' could not be found.")]
  public void LoadDataContainerWithoutTimestampColumn ()
  {
    ObjectID id = new ObjectID ("ClassWithoutTimestampProperty", new Guid ("{027DCBD7-ED68-461d-AE80-B8E145A7B816}"));

    DataContainer container = Provider.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), 
      "Invalid ClassID 'NonExistingClassID' for ID 'c9f16f93-cf42-4357-b87b-7493882aaeaf' encountered.")]
  public void LoadDataContainerWithNonExistingClassID ()
  {
    ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{C9F16F93-CF42-4357-B87B-7493882AAEAF}"));

    DataContainer container = Provider.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), "The mandatory column 'OrderNo' could not be found.")]
  public void LoadDataContainerWithClassIDFromOtherClass ()
  {
    ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{895853EB-06CD-4291-B467-160560AE8EC1}"));

    DataContainer container = Provider.LoadDataContainer (id);
  }

  [Test]
  public void LoadDataContainerByNonExistingID ()
  {
    ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

    Assert.IsNull (Provider.LoadDataContainer (id));
  }

  [Test]
  public void LoadDataContainerByID ()
  {
    ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    DataContainer actualContainer = Provider.LoadDataContainer (id);

    DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (expectedContainer, actualContainer);
  }

  [Test]
  public void LoadDerivedDataContainerByID ()
  {
    DataContainer actualContainer = Provider.LoadDataContainer (DomainObjectIDs.Partner1);
    DataContainer expectedContainer = TestDataContainerFactory.CreatePartner1DataContainer ();

    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (expectedContainer, actualContainer);
  }

  [Test]
  public void LoadTwiceDerivedDataContainerByID ()
  {
    DataContainer actualContainer = Provider.LoadDataContainer (DomainObjectIDs.Distributor2);
    DataContainer expectedContainer = TestDataContainerFactory.CreateDistributor2DataContainer ();

    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (expectedContainer, actualContainer);
  }

  [Test]
  public void LoadDataContainerWithNullForeignKey ()
  {
    ObjectID id = new ObjectID ("ClassWithValidRelations", new Guid ("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

    DataContainer container = Provider.LoadDataContainer (id);

    PropertyValue actualPropertyValue = container.PropertyValues["ClassWithGuidKeyOptional"];

    Assert.IsNotNull (actualPropertyValue, "PropertyValue");
    Assert.IsNull (actualPropertyValue.Value, "PropertyValue.Value");
  }

  [Test]
  public void LoadDataContainerWithRelation ()
  {
    DataContainer orderTicketContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
    Assert.AreEqual (DomainObjectIDs.Order1, orderTicketContainer.GetObjectID ("Order"));
  }

  [Test]
  public void LoadDataContainerWithRelationAndInheritance ()
  {
    DataContainer ceoContainer = Provider.LoadDataContainer (DomainObjectIDs.Ceo7);
    Assert.AreEqual (DomainObjectIDs.Partner2, ceoContainer.GetObjectID ("Company"));
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), 
      "Error while reading property 'Partner' for class 'ClassWithoutRelatedClassIDColumn':" 
      + " Incorrect database format encountered."
      + " Class must have column 'PartnerIDClassID' defined, because it points to derived class 'Partner'.")]
  public void LoadDataContainerWithoutRelatedIDColumn ()
  {
    ObjectID id = new ObjectID ("ClassWithoutRelatedClassIDColumn", new Guid ("{CD3BE83E-FBB7-4251-AAE4-B216485C5638}")); 

    Provider.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException), 
      "Error while reading property 'Company' for class 'ClassWithoutRelatedClassIDColumnAndDerivation':" 
      + " Incorrect database format encountered."
      + " Class must have column 'CompanyIDClassID' defined, because at least one class inherits from 'Company'.")]
  public void LoadDataContainerWithoutRelatedIDColumnAndDerivation ()
  {
    ObjectID id = new ObjectID ("ClassWithoutRelatedClassIDColumnAndDerivation", 
        new Guid ("{4821D7F7-B586-4435-B572-8A96A44B113E}")); 

    Provider.LoadDataContainer (id);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The value of the provided ObjectID is of type 'System.String', but only 'System.Guid' is supported.\r\nParameter name: id")]
  public void LoadDataContainerWithObjectIDWithValueOfInvalidType ()
  {
    ObjectID invalidID = new ObjectID (DomainObjectIDs.Customer1.ClassID, DomainObjectIDs.Customer1.Value.ToString ());

    Provider.LoadDataContainer (invalidID);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ObjectID does not match with this StorageProvider's ID 'TestDomain'.\r\nParameter name: id")]
  public void LoadDataContainerWithObjectIDWithWrongStorageProviderID ()
  {
    ObjectID invalidID = new ObjectID (DomainObjectIDs.Official1.ClassID, (int) DomainObjectIDs.Official1.Value);

    Provider.LoadDataContainer (invalidID);
  }
}
}
