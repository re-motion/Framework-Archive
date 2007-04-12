using System;
using System.Data.SqlClient;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderSaveExistingTest : SqlProviderBaseTest
  {
    [Test]
    public void Save ()
    {
      using (Provider)
      {
        DataContainer classWithAllDataTypes = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"] = ClassWithAllDataTypes.EnumType.Value2;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value2, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      }
    }

    [Test]
    public void SaveAllSimpleDataTypes ()
    {
      using (Provider)
      {
        DataContainer classWithAllDataTypes = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (false, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (85, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 1), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (2005, 1, 1, 17, 0, 0), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (123456.789, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (987654.321, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value1, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (32767, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (2147483647, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (9223372036854775807, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (6789.321, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("abcdeföäü", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage1 (classWithAllDataTypes.GetBytes ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"] = true;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"] = (byte) 42;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"] = new DateTime (1972, 10, 26);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"] = new DateTime (1974, 10, 26, 15, 17, 19);
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
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = ResourceManager.GetImage2 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (true, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
        Assert.AreEqual (42, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
        Assert.AreEqual (new DateTime (1972, 10, 26), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"]);
        Assert.AreEqual (new DateTime (1974, 10, 26, 15, 17, 19), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
        Assert.AreEqual (564.956, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
        Assert.AreEqual (5334.2456, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
        Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
        Assert.AreEqual (new Guid ("{98E0FE88-7DB4-4f6c-A1C1-86682D5C95C9}"), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
        Assert.AreEqual (67, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"]);
        Assert.AreEqual (42424242, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"]);
        Assert.AreEqual (424242424242424242, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"]);
        Assert.AreEqual (42.42, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
        Assert.AreEqual ("zyxwvuZaphodBeeblebrox", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"]);
        Assert.AreEqual ("123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876123450987612345098761234509876", classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
        ResourceManager.IsEqualToImage2 (classWithAllDataTypes.GetBytes ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      }
    }

    [Test]
    public void SaveAllNullableTypes ()
    {
      using (Provider)
      {
        DataContainer classWithAllDataTypes = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (new NaByte (78), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (new NaDecimal (new decimal (765.098)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (new NaInt16 (12000), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (new NaInt64 (3147483647), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (new NaSingle (12.456F), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);

        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = new NaBoolean (false);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = new NaByte (100);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = new NaDateTime (new DateTime (2007, 1, 18));
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = new NaDateTime (new DateTime (2005, 1, 18, 10, 10, 10));
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = new NaDecimal (new decimal (20.123));
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = new NaDouble (56.87);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = new NaGuid (new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}"));
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = new NaInt16 (-43);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = new NaInt32 (-42);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = new NaInt64 (-41);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = new NaSingle (-40F);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = ResourceManager.GetImage1 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (new NaBoolean (false), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (new NaByte (100), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2007, 1, 18)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2005, 1, 18, 10, 10, 10)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (new NaDecimal (new decimal (20.123)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (new NaDouble (56.87), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (new NaGuid (new Guid ("{10FD9EDE-F3BB-4bb9-9434-9B121C6733A0}")), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (new NaInt16 (-43), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (new NaInt32 (-42), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (new NaInt64 (-41), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (new NaSingle (-40F), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        ResourceManager.IsEqualToImage1 (classWithAllDataTypes.GetBytes ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
      }
    }

    [Test]
    public void SaveAllNullableTypesWithNull ()
    {
      // Note for NullableBinaryProperty: Because the value in the database is already null, the property has
      //  to be changed first to something different to ensure the null value is written back.
      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = ResourceManager.GetImage1 ();

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        Provider.Save (collection);
      }

      using (Provider)
      {
        DataContainer classWithAllDataTypes = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (new NaBoolean (true), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (new NaByte (78), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (new NaDecimal (new decimal (765.098)), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (new NaDouble (654321.789), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}")), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (new NaInt16 (12000), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (new NaInt32 (-2147483647), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (new NaInt64 (3147483647), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (new NaSingle (12.456F), classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        ResourceManager.IsEqualToImage1 (classWithAllDataTypes.GetBytes ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));

        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"] = NaBoolean.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"] = NaByte.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"] = NaDateTime.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"] = NaDateTime.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"] = NaDecimal.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"] = NaDouble.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"] = NaGuid.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"] = NaInt16.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"] = NaInt32.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"] = NaInt64.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"] = NaSingle.Null;
        classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (classWithAllDataTypes);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer classWithAllDataTypes = sqlProvider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);

        Assert.AreEqual (NaBoolean.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
        Assert.AreEqual (NaByte.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
        Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
        Assert.AreEqual (NaDateTime.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
        Assert.AreEqual (NaDecimal.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
        Assert.AreEqual (NaDouble.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
        Assert.AreEqual (NaGuid.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
        Assert.AreEqual (NaInt16.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
        Assert.AreEqual (NaInt32.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
        Assert.AreEqual (NaInt64.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
        Assert.AreEqual (NaSingle.Null, classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);
        Assert.IsNull (classWithAllDataTypes["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
      }
    }

    [Test]
    public void SaveWithNoChanges ()
    {
      using (Provider)
      {
        DataContainer classWithAllDataTypes = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1);
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

        Assert.AreEqual (1, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Mainboard", orderItemContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"]);

        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"] = "Raumschiff";

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);
        collection.Add (orderItemContainer);

        Provider.Save (collection);
      }


      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        DataContainer orderItemContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderItem1);

        Assert.AreEqual (10, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
        Assert.AreEqual ("Raumschiff", orderItemContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"]);
      }
    }

    [Test]
    [ExpectedException (typeof (ConcurrencyViolationException), ExpectedMessage = 
        "Concurrency violation encountered. Object 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' has already been changed by someone else.")]
    public void ConcurrentSave ()
    {
      using (Provider)
      {
        DataContainer orderContainer1 = Provider.LoadDataContainer (DomainObjectIDs.Order1);
        DataContainer orderContainer2 = Provider.LoadDataContainer (DomainObjectIDs.Order1);

        orderContainer1["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
        orderContainer2["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 11;

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
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
        collection.Add (orderContainer);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        sqlProvider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void WrapSqlException ()
    {
      using (Provider)
      {
        DataContainerCollection collection = new DataContainerCollection ();
        DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

        PropertyDefinition newDefinition =
            TestMappingConfiguration.Current.ClassDefinitions[typeof (OrderItem)]["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"];

        orderContainer.PropertyValues.Add (new PropertyValue (newDefinition, "Raumschiff"));
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"] = "Auto";

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
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
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
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
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

        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;
        orderItemContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product"] = "Raumschiff";

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

        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        Provider.BeginTransaction ();
        Provider.Save (collection);
        Provider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    public void TransactionalLoadDataContainerAndSave ()
    {
      using (Provider)
      {
        Provider.BeginTransaction ();
        DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);

        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        Provider.Save (collection);
        Provider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
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
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order",
            DomainObjectIDs.Order1);

        orderTicketContainers[0]["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"] = "C:\newFile.jpg";

        Provider.Save (orderTicketContainers);
        Provider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderTicketContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderTicket1);
        Assert.AreEqual ("C:\newFile.jpg", orderTicketContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"]);
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
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        Provider.Save (collection);
        Provider.SetTimestamp (collection);
        Provider.Commit ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        Assert.AreEqual (10, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
        Assert.IsFalse (oldTimestamp.Equals (orderContainer.Timestamp));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Commit cannot be called without calling BeginTransaction first.")]
    public void CommitWithoutBeginTransaction ()
    {
      using (Provider)
      {
        DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

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

        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderContainer);

        Provider.Save (collection);
        Provider.SetTimestamp (collection);
        Provider.Rollback ();
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Order1);
        Assert.AreEqual (1, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"]);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Rollback cannot be called without calling BeginTransaction first.")]
    public void RollbackWithoutBeginTransaction ()
    {
      using (Provider)
      {
        DataContainer orderContainer = Provider.LoadDataContainer (DomainObjectIDs.Order1);
        orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 10;

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
        OrderTicket orderTicket = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1);
        orderTicket.Order = Order.GetObject (DomainObjectIDs.Order2);

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (orderTicket.DataContainer);

        Provider.Save (collection);
      }

      // expectation: no exception
    }

    [Test]
    public void SaveForeignKeyInOtherStorageProvider ()
    {
      using (Provider)
      {
        Order order = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
        order.Official = (Official) ClientTransactionMock.GetObject (DomainObjectIDs.Official2);

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (order.DataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer orderContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.OrderWithoutOrderItem);
        Assert.AreEqual (DomainObjectIDs.Official2, orderContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndDerivedClass ()
    {
      using (Provider)
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Partner.GetObject (DomainObjectIDs.Partner1);

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (ceo.DataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer ceoContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Partner1, ceoContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveForeignKeyWithClassIDColumnAndBaseClass ()
    {
      using (Provider)
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = Supplier.GetObject (DomainObjectIDs.Supplier1);

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (ceo.DataContainer);

        Provider.Save (collection);
      }

      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer ceoContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Ceo1);
        Assert.AreEqual (DomainObjectIDs.Supplier1, ceoContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company"]);
      }
    }

    [Test]
    public void SaveNullForeignKey ()
    {
      using (Provider)
      {
        Computer computer = Computer.GetObject (DomainObjectIDs.Computer1);
        computer.Employee = null;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (computer.DataContainer);

        Provider.Save (collection);
      }


      using (SqlProvider sqlProvider = new SqlProvider (ProviderDefinition))
      {
        DataContainer computerContainer = sqlProvider.LoadDataContainer (DomainObjectIDs.Computer1);
        Assert.IsNull (computerContainer.GetObjectID ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"));
      }
    }

    [Test]
    public void SaveNullForeignKeyWithInheritance ()
    {
      using (Provider)
      {
        Ceo ceo = Ceo.GetObject (DomainObjectIDs.Ceo1);
        ceo.Company = null;

        DataContainerCollection collection = new DataContainerCollection ();
        collection.Add (ceo.DataContainer);

        Provider.Save (collection);
      }

      using (SqlConnection connection = new SqlConnection (c_connectionString))
      {
        connection.Open ();
        using (SqlCommand command = new SqlCommand ("select * from Ceo where ID = @id", connection))
        {
          command.Parameters.AddWithValue ("@id", DomainObjectIDs.Ceo1.Value);
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
        ExpectedMessage = "Cannot call BeginTransaction when a transaction is already in progress.")]
    public void CallBeginTransactionTwice ()
    {
      using (Provider)
      {
        Provider.BeginTransaction ();
        Provider.BeginTransaction ();
      }
    }
  }
}
