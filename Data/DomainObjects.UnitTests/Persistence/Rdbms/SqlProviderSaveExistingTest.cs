using System;
using System.Data.SqlClient;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;

using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderSaveExistingTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderSaveExistingTest ()
  {
  }

  // methods and properties

  [Test]
  public void Save ()
  {
    using (Provider)
    {
      DataContainer classWithAllDataTypes = Provider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["EnumProperty"]);
      classWithAllDataTypes["EnumProperty"] = ClassWithAllDataTypes.EnumType.Value2;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes["EnumProperty"]);
    }
  }

  [Test]
  public void SaveAllSimpleDataTypes ()
  {
    using (Provider)
    {
      DataContainer classWithAllDataTypes = Provider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (false, classWithAllDataTypes["BooleanProperty"]);
      Assert.AreEqual (85, classWithAllDataTypes["ByteProperty"]);
      Assert.AreEqual ('a', classWithAllDataTypes["CharProperty"]);
      Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes["DateProperty"]);
      Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes["DateTimeProperty"]);
      Assert.AreEqual (123456.789, classWithAllDataTypes["DecimalProperty"]);
      Assert.AreEqual (987654.321, classWithAllDataTypes["DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["EnumProperty"]);
      Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes["GuidProperty"]);
      Assert.AreEqual (32767, classWithAllDataTypes["Int16Property"]);
      Assert.AreEqual (2147483647, classWithAllDataTypes["Int32Property"]);
      Assert.AreEqual (9223372036854775807, classWithAllDataTypes["Int64Property"]);
      Assert.AreEqual (6789.321, classWithAllDataTypes["SingleProperty"]);
      Assert.AreEqual ("abcdeföäü", classWithAllDataTypes["StringProperty"]);

      classWithAllDataTypes["BooleanProperty"] = true;
      classWithAllDataTypes["ByteProperty"] = (byte) 42;
      classWithAllDataTypes["CharProperty"] = 'z';
      classWithAllDataTypes["DateProperty"] = new DateTime (1972, 10, 26);
      classWithAllDataTypes["DateTimeProperty"] = new DateTime (1974, 10, 26, 15, 17, 19);
      classWithAllDataTypes["DecimalProperty"] = (decimal) 564.956;
      classWithAllDataTypes["DoubleProperty"] = 5334.2456;
      classWithAllDataTypes["EnumProperty"] = ClassWithAllDataTypes.EnumType.Value0;
      classWithAllDataTypes["GuidProperty"] = new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}");
      classWithAllDataTypes["Int16Property"] = (short) 67;
      classWithAllDataTypes["Int32Property"] = 42424242;
      classWithAllDataTypes["Int64Property"] = 424242424242424242;
      classWithAllDataTypes["SingleProperty"] = (float) 42.42;
      classWithAllDataTypes["StringProperty"] = "zyxwvuZaphodBeeblebrox";

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (true, classWithAllDataTypes["BooleanProperty"]);
      Assert.AreEqual (42, classWithAllDataTypes["ByteProperty"]);
      Assert.AreEqual ('z', classWithAllDataTypes["CharProperty"]);
      Assert.AreEqual (new DateTime (1972, 10, 26), classWithAllDataTypes["DateProperty"]);
      Assert.AreEqual (new DateTime (1974, 10, 26, 15, 17, 19), classWithAllDataTypes["DateTimeProperty"]);
      Assert.AreEqual (564.956, classWithAllDataTypes["DecimalProperty"]);
      Assert.AreEqual (5334.2456, classWithAllDataTypes["DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["EnumProperty"]);
      Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["GuidProperty"]);
      Assert.AreEqual (67, classWithAllDataTypes["Int16Property"]);
      Assert.AreEqual (42424242, classWithAllDataTypes["Int32Property"]);
      Assert.AreEqual (424242424242424242, classWithAllDataTypes["Int64Property"]);
      Assert.AreEqual (42.42, classWithAllDataTypes["SingleProperty"]);
      Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["StringProperty"]);
    }
  }

  [Test]
  public void SaveAllNullableTypes ()
  {
    using (Provider)
    {
      DataContainer classWithAllDataTypes = Provider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes["NaInt32Property"]);

      classWithAllDataTypes["NaBooleanProperty"] = new NaBoolean (false);
      classWithAllDataTypes["NaDateProperty"] = new NaDateTime (new DateTime (2007, 1, 18));
      classWithAllDataTypes["NaDateTimeProperty"] = new NaDateTime (new DateTime (2005, 1, 18, 10, 10, 10));
      classWithAllDataTypes["NaDoubleProperty"] = new NaDouble (56.87);
      classWithAllDataTypes["NaInt32Property"] = new NaInt32 (-42);

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (new NaBoolean (false), classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2007, 1, 18)), classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 1, 18, 10, 10, 10)), classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (new NaDouble (56.87), classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (new NaInt32 (-42), classWithAllDataTypes["NaInt32Property"]);
    }
  }

  [Test]
  public void SaveAllNullableTypesWithNull ()
  {
    using (Provider)
    {
      DataContainer classWithAllDataTypes = Provider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes["NaInt32Property"]);

      classWithAllDataTypes["NaBooleanProperty"] = NaBoolean.Null;
      classWithAllDataTypes["NaDateProperty"] = NaDateTime.Null;
      classWithAllDataTypes["NaDateTimeProperty"] = NaDateTime.Null;
      classWithAllDataTypes["NaDoubleProperty"] = NaDouble.Null;
      classWithAllDataTypes["NaInt32Property"] = NaInt32.Null;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (GetClassWithAllDataTypesID ());

      Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes["NaBooleanProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateProperty"]);
      Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["NaDateTimeProperty"]);
      Assert.AreEqual (NaDouble.Null, classWithAllDataTypes["NaDoubleProperty"]);
      Assert.AreEqual (NaInt32.Null, classWithAllDataTypes["NaInt32Property"]);
    }
  }

  [Test]
  public void SaveWithNoChanges ()
  {
    using (Provider)
    {
      DataContainer classWithAllDataTypes = Provider.LoadDataContainer (GetClassWithAllDataTypesID ());
      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (classWithAllDataTypes);

      Provider.Save (collection);
    }

    // expectation: no exception
  }


  [Test]
  public void SaveMultipleDataContainers ()
  {
    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderItemContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderItem1);

      Assert.AreEqual (1, orderContainer["OrderNumber"]);
      Assert.AreEqual ("Mainboard", orderItemContainer["Product"]);

      orderContainer["OrderNumber"] = 10;
      orderItemContainer["Product"] = "Raumschiff";

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);
      collection.Add (orderItemContainer);

      Provider.Save (collection);
    }


    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderItemContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderItem1);

      Assert.AreEqual (10, orderContainer["OrderNumber"]);
      Assert.AreEqual ("Raumschiff", orderItemContainer["Product"]);
    }
  }

  [Test]
  [ExpectedException (typeof (ConcurrencyViolationException), 
      "Concurrency violation encountered. Object"
      + " 'TestDomain|Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'"
      + " has already been changed by someone else.")]
  public void ConcurrentSave ()
  {
    using (Provider)
    {
      DataContainer orderContainer1 = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      DataContainer orderContainer2 = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      orderContainer1["OrderNumber"] = 10;
      orderContainer2["OrderNumber"] = 11;

      DataContainerCollection collection1 = new DataContainerCollection ();
      collection1.Add (orderContainer1);
      Provider.Save (collection1);

      DataContainerCollection collection2 = new DataContainerCollection ();
      collection2.Add (orderContainer2);
      Provider.Save (collection2);      
    }
  }

  [Test]
  public void SaveWithConnect ()
  {
    DataContainerCollection collection = new DataContainerCollection ();

    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      orderContainer["OrderNumber"] = 10;
      collection.Add (orderContainer);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      sqlProvider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, orderContainer["OrderNumber"]);
    }
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException))]
  public void WrapSqlException ()
  {
    using (Provider)
    {
      DataContainerCollection collection = new DataContainerCollection ();
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      PropertyDefinition newDefinition = 
          TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderItem)]["Product"];

      orderContainer.PropertyValues.Add (new PropertyValue (newDefinition, "Raumschiff"));
      orderContainer["Product"] = "Auto";

      collection.Add (orderContainer);
      Provider.Save (collection);
    }
  }

  [Test]
  public void SetTimestamp ()
  {
    object oldTimestamp = null;
    using (Provider)
    {
      DataContainerCollection collection = new DataContainerCollection ();
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      oldTimestamp = orderContainer.Timestamp;
      orderContainer["OrderNumber"] = 10;
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.SetTimestamp (collection);

      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }
  }

  [Test]
  public void SetTimestampWithConnect ()
  {
    DataContainerCollection collection = new DataContainerCollection ();
    DataContainer orderContainer = null;
    object oldTimestamp = null;

    using (Provider)
    {
      orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      oldTimestamp = orderContainer.Timestamp;
      orderContainer["OrderNumber"] = 10;
      collection.Add (orderContainer);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      sqlProvider.SetTimestamp (collection);
    }

    Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
  }

  [Test]
  public void SetTimestampForMultipleDataContainers ()
  {
    object oldOrderTimestamp = null;
    object oldOrderItemTimestamp = null;

    DataContainer orderContainer = null;
    DataContainer orderItemContainer = null;

    using (Provider)
    {
      orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      orderItemContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderItem1);

      oldOrderTimestamp = orderContainer.Timestamp;
      oldOrderItemTimestamp = orderItemContainer.Timestamp;

      orderContainer["OrderNumber"] = 10;
      orderItemContainer["Product"] = "Raumschiff";

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);
      collection.Add (orderItemContainer);

      Provider.Save (collection);
      Provider.SetTimestamp (collection);
    }

    Assert.IsFalse (oldOrderTimestamp.Equals (orderContainer.Timestamp));
    Assert.IsFalse (oldOrderItemTimestamp.Equals (orderItemContainer.Timestamp));
  }

  [Test]
  public void TransactionalSave ()
  {
    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.BeginTransaction ();
      Provider.Save (collection);
      Provider.Commit ();
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, orderContainer["OrderNumber"]);
    }
  }

  [Test]
  public void TransactionalLoadDataContainerAndSave ()
  {
    using (Provider)
    {
      Provider.BeginTransaction ();
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.Commit ();
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, orderContainer["OrderNumber"]);
    }
  }


  [Test]
  public void TransactionalLoadDataContainersByRelatedIDAndSave ()
  {
    using (Provider)
    {
      Provider.BeginTransaction ();

      DataContainerCollection orderTicketContainers = Provider.LoadDataContainersByRelatedID (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderTicket)],
          "Order",
          DomainObjectIDs.Order1);

      orderTicketContainers[0]["FileName"] = "C:\newFile.jpg";

      Provider.Save (orderTicketContainers);
      Provider.Commit ();
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderTicketContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
      Assert.AreEqual ("C:\newFile.jpg", orderTicketContainer["FileName"]);
    }
  }

  [Test]
  public void TransactionalSaveAndSetTimestamp ()
  {
    object oldTimestamp = null;
    using (Provider)
    {
      Provider.BeginTransaction ();
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      oldTimestamp = orderContainer.Timestamp;
      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.SetTimestamp (collection);
      Provider.Commit ();
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (10, orderContainer["OrderNumber"]);
      Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
    }
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Commit cannot be called without calling BeginTransaction first.")]
  public void CommitWithoutBeginTransaction ()
  {
    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.Commit ();
    }
  }

  [Test]
  public void SaveWithRollback ()
  {
    using (Provider)
    {
      Provider.BeginTransaction ();
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.SetTimestamp (collection);
      Provider.Rollback ();
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
      Assert.AreEqual (1, orderContainer["OrderNumber"]);
    }
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Rollback cannot be called without calling BeginTransaction first.")]
  public void RollbackWithoutBeginTransaction ()
  {
    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
      orderContainer["OrderNumber"] = 10;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
      Provider.Rollback ();
    }
  }

  [Test]
  public void SaveForeignKeyInSameStorageProvider ()
  {
    using (Provider)
    {
      DataContainer orderTicketContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
      orderTicketContainer["Order"] = DomainObjectIDs.Order2;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderTicketContainer);

      Provider.Save (collection);
    }

    // expectation: no exception
  }

  [Test]
  public void SaveForeignKeyInOtherStorageProvider ()
  {
    using (Provider)
    {
      DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      orderContainer["Official"] = DomainObjectIDs.Official2;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
      Assert.AreEqual (DomainObjectIDs.Official2, orderContainer["Official"]);
    }    
  }

  [Test]
  public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
  {
    using (Provider)
    {
      DataContainer ceoContainer = Provider.LoadDataContainer (DomainObjectIDs.Ceo1);
      ceoContainer["Company"] = DomainObjectIDs.Partner1;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (ceoContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer ceoContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Partner1, ceoContainer["Company"]);
    }    
  }

  [Test]
  public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
  {
    using (Provider)
    {
      DataContainer ceoContainer = Provider.LoadDataContainer (DomainObjectIDs.Ceo1);
      ceoContainer["Company"] = DomainObjectIDs.Supplier1;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (ceoContainer);

      Provider.Save (collection);
    }

    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer ceoContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Ceo1);
      Assert.AreEqual (DomainObjectIDs.Supplier1, ceoContainer["Company"]);
    }    
  }

  [Test]
  public void SaveNullForeignKey ()
  {
    using (Provider)
    {
      DataContainer orderTicketContainer = Provider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
      orderTicketContainer["Order"] = null;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (orderTicketContainer);

      Provider.Save (collection);
    }


    using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
    {
      DataContainer orderTicketContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
      Assert.IsNull (orderTicketContainer.GetObjectID ("Order"));
    }        
  }

  [Test]
  public void SaveNullForeignKeyWithInheritance ()
  {
    using (Provider)
    {
      DataContainer ceoContainer = Provider.LoadDataContainer (DomainObjectIDs.Ceo1);
      ceoContainer["Company"] = null;

      DataContainerCollection collection = new DataContainerCollection ();
      collection.Add (ceoContainer);

      Provider.Save (collection);
    }

    using (SqlConnection connection = new SqlConnection (c_connectionString))
    {
      connection.Open ();
      using (SqlCommand command = new SqlCommand ("select * from Ceo where ID = @id", connection))
      {
        command.Parameters.Add ("@id", DomainObjectIDs.Ceo1.Value);
        using (SqlDataReader reader = command.ExecuteReader ())
        {
          reader.Read ();
          int columnOrdinal = reader.GetOrdinal ("CompanyIDClassID");
          Assert.IsTrue (reader.IsDBNull (columnOrdinal));
        }
      }
    }
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      "Cannot call BeginTransaction when a transaction is already in progress.")]
  public void CallBeginTransactionTwice ()
  {
    using (Provider)
    {
      Provider.BeginTransaction ();
      Provider.BeginTransaction ();
    }
  }

  private ObjectID GetClassWithAllDataTypesID ()
  {
    Guid idGuid = new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}");
    return new ObjectID (c_testDomainProviderID, "ClassWithAllDataTypes", idGuid);
  }
}
}
