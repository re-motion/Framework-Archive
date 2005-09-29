using System;
using System.Data.SqlClient;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
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
        TestMappingConfiguration.Current.ClassDefinitions["Order"]);

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
    classWithAllDataTypes["BinaryProperty"] = ResourceManager.GetImage1 ();

    classWithAllDataTypes["NaBooleanProperty"] = new NaBoolean (false);
    classWithAllDataTypes["NaByteProperty"] = new NaByte (21);
    classWithAllDataTypes["NaDateProperty"] = new NaDateTime (new DateTime (2007, 1, 18));
    classWithAllDataTypes["NaDateTimeProperty"] = new NaDateTime (new DateTime (2005, 1, 18, 11, 11, 11));
    classWithAllDataTypes["NaDecimalProperty"] = new NaDecimal (new decimal (50));
    classWithAllDataTypes["NaDoubleProperty"] = new NaDouble (56.87);
    classWithAllDataTypes["NaGuidProperty"] = new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
    classWithAllDataTypes["NaInt16Property"] = new NaInt16 (51);
    classWithAllDataTypes["NaInt32Property"] = new NaInt32 (52);
    classWithAllDataTypes["NaInt64Property"] = new NaInt64 (53);
    classWithAllDataTypes["NaSingleProperty"] = new NaSingle (54F);

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (classWithAllDataTypes);

    Provider.Save (collection);

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      classWithAllDataTypes = Provider.LoadDataContainer (newID);

      Assert.AreEqual (true, classWithAllDataTypes["BooleanProperty"]);
      Assert.AreEqual (42, classWithAllDataTypes["ByteProperty"]);
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
      ResourceManager.IsEqualToImage1 (classWithAllDataTypes.GetBytes ("BinaryProperty"));

      Assert.AreEqual (new NaBoolean (false), classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (new NaByte (21), classWithAllDataTypes["NaByteProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2007, 1, 18)), classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 1, 18, 11, 11, 11)), classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (new NaDecimal (new decimal (50)), classWithAllDataTypes["NaDecimalProperty"]);
      Assert.AreEqual (new NaDouble (56.87), classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), classWithAllDataTypes["NaGuidProperty"]);
      Assert.AreEqual (new NaInt16 (51), classWithAllDataTypes["NaInt16Property"]);
      Assert.AreEqual (new NaInt32 (52), classWithAllDataTypes["NaInt32Property"]);
      Assert.AreEqual (new NaInt64 (53), classWithAllDataTypes["NaInt64Property"]);
      Assert.AreEqual (new NaSingle (54F), classWithAllDataTypes["NaSingleProperty"]);

      Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes["NaBooleanWithNullValueProperty"]);
      Assert.AreEqual (NaByte.Null, classWithAllDataTypes["NaByteWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateTimeWithNullValueProperty"]);
      Assert.AreEqual (NaDecimal.Null, classWithAllDataTypes["NaDecimalWithNullValueProperty"]);
      Assert.AreEqual (NaDouble.Null, classWithAllDataTypes["NaDoubleWithNullValueProperty"]);
      Assert.AreEqual (NaGuid.Null, classWithAllDataTypes["NaGuidWithNullValueProperty"]);
      Assert.AreEqual (NaInt16.Null, classWithAllDataTypes["NaInt16WithNullValueProperty"]);
      Assert.AreEqual (NaInt32.Null, classWithAllDataTypes["NaInt32WithNullValueProperty"]);
      Assert.AreEqual (NaInt64.Null, classWithAllDataTypes["NaInt64WithNullValueProperty"]);
      Assert.AreEqual (NaSingle.Null, classWithAllDataTypes["NaSingleWithNullValueProperty"]);
      Assert.IsNull (classWithAllDataTypes["StringWithNullValueProperty"]);
      Assert.IsNull (classWithAllDataTypes["NullableBinaryProperty"]);
    }
  }

  [Test]
  public void ExistingObjectRelatesToNew ()
  {
    ClassDefinition employeeClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Employee)];
    Employee newSupervisor = new Employee ();
    Employee existingSubordinate = Employee.GetObject (DomainObjectIDs.Employee1);

    newSupervisor.Name = "Supervisor";
    existingSubordinate.Supervisor = newSupervisor;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (existingSubordinate.DataContainer);
    collection.Add (newSupervisor.DataContainer);

    Provider.Save (collection);

    DataContainer newSupervisorContainer = Provider.LoadDataContainer (newSupervisor.ID);
    DataContainer existingSubordinateContainer = Provider.LoadDataContainer (existingSubordinate.ID);

    Assert.IsNotNull (newSupervisorContainer);
    Assert.AreEqual (newSupervisorContainer.ID, existingSubordinateContainer.GetObjectID ("Supervisor"));
  }

  [Test]
  public void NewObjectRelatesToExisting ()
  {
    Order order = new Order ();
    order.DeliveryDate = new DateTime (2005, 12, 24);
    order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);

    ObjectID newObjectID = order.ID;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (order.DataContainer);

    Provider.Save (collection);

    DataContainer loadedDataContainer = Provider.LoadDataContainer (newObjectID);

    Assert.IsNotNull (loadedDataContainer);
    Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetObjectID ("Customer"));
  }

  [Test]
  public void NewRelatedObjects ()
  {
    Customer newCustomer = new Customer ();
    Order newOrder = new Order ();

    newOrder.DeliveryDate = new DateTime (2005, 12, 24);
    newOrder.Customer = newCustomer;

    DataContainerCollection collection = new DataContainerCollection ();
    collection.Add (newOrder.DataContainer);
    collection.Add (newCustomer.DataContainer);

    Provider.Save (collection);

    DataContainer newCustomerContainer = Provider.LoadDataContainer (newCustomer.ID);
    DataContainer newOrderContainer = Provider.LoadDataContainer (newOrder.ID);

    Assert.IsNotNull (newCustomerContainer);
    Assert.IsNotNull (newOrderContainer);
    Assert.AreEqual (newCustomerContainer.ID, newOrderContainer.GetObjectID ("Customer"));
  }

  [Test]
  public void SaveNullBinary ()
  {
    ObjectID newID;
    using (Provider)
    {
      DataContainer dataContainer = Provider.CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
      newID = dataContainer.ID;
      
      SetDefaultValues (dataContainer);      
      dataContainer["NullableBinaryProperty"] = null;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (dataContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
      Assert.IsNull (dataContainer["NullableBinaryProperty"]);
    }    
  }

  [Test]
  public void SaveEmptyBinary ()
  {
    ObjectID newID;
    using (Provider)
    {
      DataContainer dataContainer = Provider.CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
      newID = dataContainer.ID;
      
      SetDefaultValues (dataContainer);
      dataContainer["NullableBinaryProperty"] = new byte[0];

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (dataContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
      ResourceManager.IsEmptyImage (dataContainer.GetBytes ("NullableBinaryProperty"));
    }    
  }

  [Test]
  public void SaveLargeBinary ()
  {
    ObjectID newID;
    using (Provider)
    {
      DataContainer dataContainer = Provider.CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)]);
      newID = dataContainer.ID;
      
      SetDefaultValues (dataContainer);
      dataContainer["BinaryProperty"] = ResourceManager.GetImageLarger1MB ();

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (dataContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
      ResourceManager.IsEqualToImageLarger1MB (dataContainer.GetBytes ("BinaryProperty"));
    }    
  }

  private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
  {
    // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
    classWithAllDataTypesContainer["DateProperty"] = DateTime.Now;
    classWithAllDataTypesContainer["DateTimeProperty"] = DateTime.Now;

    // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
    classWithAllDataTypesContainer["DecimalProperty"] = 10m;
  }
}
}
