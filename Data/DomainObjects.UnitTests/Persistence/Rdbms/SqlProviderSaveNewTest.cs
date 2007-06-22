using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderSaveNewTest : SqlProviderBaseTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    [Test]
    public void NewDataContainer ()
    {
      DataContainer newDataContainer = Provider.CreateNewDataContainer (TestMappingConfiguration.Current.ClassDefinitions["Computer"]);

      newDataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.SerialNumber"] = "123";

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
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];

      DataContainer classWithAllDataTypes = Provider.CreateNewDataContainer (classDefinition);
      ObjectID newID = classWithAllDataTypes.ID;

      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1974, 10, 25);
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (1974, 10, 26, 18, 9, 18);
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = (decimal) 564.956;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"] = 5334.2456;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"] = ClassWithAllDataTypes.EnumType.Value0;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"] = new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"] = (short) 67;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"] = 42424242;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"] = 424242424242424242;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"] = (float) 42.42;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"] = "zyxwvuZaphodBeeblebrox";
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"] = "123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876";
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage1 ();

      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = false;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = (byte) 21;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new DateTime (2007, 1, 18);
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new DateTime (2005, 1, 18, 11, 11, 11);
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = 50m;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = 56.87d;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"] = ClassWithAllDataTypes.EnumType.Value1;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}");
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = (short) 51;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = 52;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = 53L;
      classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = 54F;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        classWithAllDataTypes = Provider.LoadDataContainer (newID);

        Assert.AreEqual (true, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (42, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 25), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 26, 18, 9, 18), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (564.956, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (5334.2456d, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (67, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (42424242, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (424242424242424242, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (42.42, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage1 ((byte[]) classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

        Assert.AreEqual (false, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (21, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new DateTime (2007, 1, 18), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 18, 11, 11, 11), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (50m, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (56.87d, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumProperty"]);
        Assert.AreEqual (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (51, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (52, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (53, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (54F, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaEnumWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void ExistingObjectRelatesToNew ()
    {
      ClassDefinition employeeClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Employee)];
      Employee newSupervisor = Employee.NewObject ();
      Employee existingSubordinate = Employee.GetObject (DomainObjectIDs.Employee1);

      newSupervisor.Name = "Supervisor";
      existingSubordinate.Supervisor = newSupervisor;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (existingSubordinate.InternalDataContainer);
			collection.Add (newSupervisor.InternalDataContainer);

      Provider.Save (collection);

      DataContainer newSupervisorContainer = Provider.LoadDataContainer (newSupervisor.ID);
      DataContainer existingSubordinateContainer = Provider.LoadDataContainer (existingSubordinate.ID);

      Assert.IsNotNull (newSupervisorContainer);
      Assert.AreEqual (newSupervisorContainer.ID, existingSubordinateContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor"));
    }

    [Test]
    public void NewObjectRelatesToExisting ()
    {
      Order order = Order.NewObject ();
      order.DeliveryDate = new DateTime (2005, 12, 24);
      order.Customer = Customer.GetObject (DomainObjectIDs.Customer1);
      order.Official = Official.GetObject (DomainObjectIDs.Official1);

      ObjectID newObjectID = order.ID;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (order.InternalDataContainer);

      Provider.Save (collection);

      DataContainer loadedDataContainer = Provider.LoadDataContainer (newObjectID);

      Assert.IsNotNull (loadedDataContainer);
      Assert.AreEqual (DomainObjectIDs.Customer1, loadedDataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.AreEqual (DomainObjectIDs.Official1, loadedDataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"));
    }

    [Test]
    public void NewRelatedObjects ()
    {
      Customer newCustomer = Customer.NewObject ();
      Order newOrder = Order.NewObject ();
      Official existingOfficial = Official.GetObject (DomainObjectIDs.Official1);

      newOrder.DeliveryDate = new DateTime (2005, 12, 24);
      newOrder.Customer = newCustomer;
      newOrder.Official = existingOfficial;

      DataContainerCollection collection = new DataContainerCollection ();
			collection.Add (newOrder.InternalDataContainer);
			collection.Add (newCustomer.InternalDataContainer);

      Provider.Save (collection);

      DataContainer newCustomerContainer = Provider.LoadDataContainer (newCustomer.ID);
      DataContainer newOrderContainer = Provider.LoadDataContainer (newOrder.ID);

      Assert.IsNotNull (newCustomerContainer);
      Assert.IsNotNull (newOrderContainer);
      Assert.AreEqual (newCustomerContainer.ID, newOrderContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"));
      Assert.AreEqual (DomainObjectIDs.Official1, newOrderContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"));
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
        dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        Assert.IsNull (dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
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
        dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = new byte[0];

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        ResourceManager.IsEmptyImage ((byte[]) dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
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
        dataContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImageLarger1MB ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (dataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer dataContainer = sqlProvider.LoadDataContainer (newID);
        ResourceManager.IsEqualToImageLarger1MB ((byte[]) dataContainer.GetValue ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      }
    }

    private void SetDefaultValues (DataContainer classWithAllDataTypesContainer)
    {
      // Note: Date properties must be set, because SQL Server only accepts dates past 1/1/1753.
      classWithAllDataTypesContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"] = DateTime.Now;
      classWithAllDataTypesContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = DateTime.Now;

      // Note: SqlDecimal has problems with Decimal.MinValue => Set this property too.
      classWithAllDataTypesContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"] = 10m;
    }
  }
}
