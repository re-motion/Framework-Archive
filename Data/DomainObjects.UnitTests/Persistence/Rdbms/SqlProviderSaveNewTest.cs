using System;
using System.Data.SqlClient;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderSaveNewTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderSaveNewTest ()
  {
  }

  // methods and properties

  [Test]
  public void NewDataContainer ()
  {
    DataContainer newDataContainer = Provider.CreateNewDataContainer (
        TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order"));

    newDataContainer["DeliveryDate"] = new DateTime (2005, 1, 5);

    ObjectID newObjectID = newDataContainer.ID;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (newDataContainer);

    Provider.Save (collection);

    DataContainer loadedDataContainer = Provider.LoadDataContainer (newObjectID);

    Assert.IsNotNull (loadedDataContainer);
    Assert.AreEqual (newDataContainer.ID, loadedDataContainer.ID);
  }


  [Test]
  public void AllDataTypes ()
  {
    ClassDefinition classDefinition = 
        TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];

    DataContainer classWithAllDataTypes = Provider.CreateNewDataContainer (classDefinition);
    ObjectID newID = classWithAllDataTypes.ID;

    classWithAllDataTypes["BooleanProperty"] = true;
    classWithAllDataTypes["ByteProperty"] = (byte) 42;
    classWithAllDataTypes["CharProperty"] = 'z';
    classWithAllDataTypes["DateProperty"] = new DateTime (1974, 10, 25);
    classWithAllDataTypes["DateTimeProperty"] = new DateTime (1974, 10, 26, 18, 9, 18);
    classWithAllDataTypes["DecimalProperty"] = (decimal) 564.956;
    classWithAllDataTypes["DoubleProperty"] = 5334.2456;
    classWithAllDataTypes["EnumProperty"] = ClassWithAllDataTypes.EnumType.Value0;
    classWithAllDataTypes["GuidProperty"] = new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
    classWithAllDataTypes["Int16Property"] = (short) 67;
    classWithAllDataTypes["Int32Property"] = 42424242;
    classWithAllDataTypes["Int64Property"] = 424242424242424242;
    classWithAllDataTypes["SingleProperty"] = (float) 42.42;
    classWithAllDataTypes["StringProperty"] = "zyxwvuZaphodBeeblebrox";
    classWithAllDataTypes["NaBooleanProperty"] = new NaBoolean (false);
    classWithAllDataTypes["NaDateProperty"] = new NaDateTime (new DateTime (2007, 1, 18));
    classWithAllDataTypes["NaDateTimeProperty"] = new NaDateTime (new DateTime (2005, 1, 18, 11, 11, 11));
    classWithAllDataTypes["NaDoubleProperty"] = new NaDouble (56.87);
    classWithAllDataTypes["NaInt32Property"] = new NaInt32 (-42);

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (classWithAllDataTypes);

    Provider.Save (collection);

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      classWithAllDataTypes = Provider.LoadDataContainer (newID);

      Assert.AreEqual (true, classWithAllDataTypes["BooleanProperty"]);
      Assert.AreEqual (42, classWithAllDataTypes["ByteProperty"]);
      Assert.AreEqual ('z', classWithAllDataTypes["CharProperty"]);
      Assert.AreEqual (new DateTime (1974, 10, 25), classWithAllDataTypes["DateProperty"]);
      Assert.AreEqual (new DateTime (1974, 10, 26, 18, 9, 18), classWithAllDataTypes["DateTimeProperty"]);
      Assert.AreEqual (564.956, classWithAllDataTypes["DecimalProperty"]);
      Assert.AreEqual (5334.2456, classWithAllDataTypes["DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["EnumProperty"]);
      Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["GuidProperty"]);
      Assert.AreEqual (67, classWithAllDataTypes["Int16Property"]);
      Assert.AreEqual (42424242, classWithAllDataTypes["Int32Property"]);
      Assert.AreEqual (424242424242424242, classWithAllDataTypes["Int64Property"]);
      Assert.AreEqual (42.42, classWithAllDataTypes["SingleProperty"]);
      Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["StringProperty"]);
      Assert.AreEqual (new NaBoolean (false), classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2007, 1, 18)), classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 1, 18, 11, 11, 11)), classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (new NaDouble (56.87), classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (new NaInt32 (-42), classWithAllDataTypes["NaInt32Property"]);

      Assert.IsNull (classWithAllDataTypes["StringWithNullValueProperty"]);
      Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes["NaBooleanWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateTimeWithNullValueProperty"]);
      Assert.AreEqual (NaDouble.Null, classWithAllDataTypes["NaDoubleWithNullValueProperty"]);
      Assert.AreEqual (NaInt32.Null, classWithAllDataTypes["NaInt32WithNullValueProperty"]);
    }
  }

  [Test]
  public void ExistingObjectRelatesToNew ()
  {
    ClassDefinition employeeClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Employee)];
    DataContainer newSupervisor = Provider.CreateNewDataContainer (employeeClass);
    DataContainer existingSubordinate = Provider.LoadDataContainer (DomainObjectIDs.Employee1);

    newSupervisor["Name"] = "Supervisor";
    existingSubordinate["Supervisor"] = newSupervisor.ID;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (existingSubordinate);
    collection.Add (newSupervisor);

    Provider.Save (collection);

    newSupervisor = Provider.LoadDataContainer (newSupervisor.ID);
    existingSubordinate = Provider.LoadDataContainer (existingSubordinate.ID);

    Assert.IsNotNull (newSupervisor);
    Assert.AreEqual (newSupervisor.ID, existingSubordinate.GetObjectID ("Supervisor"));
  }

  [Test]
  public void NewObjectRelatesToExisting ()
  {
    DataContainer newDataContainer = Provider.CreateNewDataContainer (
        TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order"));

    newDataContainer["DeliveryDate"] = new DateTime (2005, 12, 24);

    newDataContainer["Customer"] = DomainObjectIDs.Customer1;

    ObjectID newObjectID = newDataContainer.ID;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (newDataContainer);

    Provider.Save (collection);

    DataContainer loadedDataContainer = Provider.LoadDataContainer (newObjectID);

    Assert.IsNotNull (loadedDataContainer);
    Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetObjectID ("Customer"));
  }

  [Test]
  public void NewRelatedObjects ()
  {
    DataContainer newCustomer = Provider.CreateNewDataContainer (
        TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Customer"));

    DataContainer newOrder = Provider.CreateNewDataContainer (
        TestMappingConfiguration.Current.ClassDefinitions.GetByClassID ("Order"));

    newOrder["DeliveryDate"] = new DateTime (2005, 12, 24);

    newOrder["Customer"] = newCustomer.ID;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (newOrder);
    collection.Add (newCustomer);

    Provider.Save (collection);

    newCustomer = Provider.LoadDataContainer (newCustomer.ID);
    newOrder = Provider.LoadDataContainer (newOrder.ID);

    Assert.IsNotNull (newCustomer);
    Assert.IsNotNull (newOrder);
    Assert.AreEqual (newCustomer.ID, newOrder.GetObjectID ("Customer"));
  }

}
}
